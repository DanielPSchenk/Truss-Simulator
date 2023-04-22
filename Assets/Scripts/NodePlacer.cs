using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NodePlacer : MonoBehaviour
{
    List<Node> nodes;
    List<Beam> beams;

    public Node nodePrefab;
    public Beam beamPrefab;


    public Node firstNode = null;

    Beam hoveringBeam = null;
    Node hoveringNode = null;
    DirectionalConstraint activeConstraint;

    public Sprite circle;
    public Sprite slider;
    public Sprite twoValue;
    
    public bool grid = true;

    public float snapRadius = .5f;
    public float gridSize = 1;

    public Gradient vis;

    public bool construct = true;

    public TMP_Text status;
    public TMP_Text warning;
    public TMP_Text hint;

    // Start is called before the first frame update
    void Start()
    {
        nodes = new List<Node>();
        beams = new List<Beam>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        Vector3 mousePos3D = new Vector3(mousePos.x, mousePos.y, 0);
        Vector2 spos = selectPos(mousePos);
        Vector3 selectPos3D = new Vector3(spos.x, spos.y, 0);

        setHint();

        if (Input.GetKeyDown(KeyCode.Delete)) {
            deleteAll();
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            switchMode();
        }

        if (construct)
        {
            if (Input.GetKeyDown(KeyCode.G)) grid = !grid;

            if (firstNode == null)
            {
                if (hoveringNode == null)
                {
                    hoveringNode = Instantiate(nodePrefab);
                }
                hoveringNode.transform.position = spos;


                //place first Node
                if (Input.GetKeyDown(KeyCode.Mouse0) && !Input.GetKey(KeyCode.Mouse1))
                {
                    Destroy(hoveringNode.gameObject);
                    firstNode = createOrSelectNode(mousePos);
                    hoveringBeam = Instantiate(beamPrefab);
                    positionHoveringBeam(selectPos3D);
                }



                //rotate constraint
                if (activeConstraint != null)
                {
                    Vector2 dir = mousePos3D - activeConstraint.transform.position;
                    dir = angleSnapping(dir);

                    Vector2 upDir = (dir).normalized;
                    activeConstraint.normalDirection = upDir;
                    activeConstraint.transform.up = upDir;
                }

                //add constraint
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    if (activeConstraint != null) activeConstraint = null;
                    else
                    {
                        Node n = selectNode(mousePos);
                        if (n != null)
                        {

                            SpriteRenderer sp = n.GetComponent<SpriteRenderer>();
                            switch (n.constraints.Count)
                            {
                                case 0:

                                    sp.sprite = slider;
                                    DirectionalConstraint c = n.gameObject.AddComponent(typeof(DirectionalConstraint)) as DirectionalConstraint;
                                    n.constraints.Add(c);
                                    activeConstraint = c;
                                    break;
                                case 1:

                                    sp.sprite = twoValue;
                                    Vector2 otherDir = Vector3.Cross(n.constraints[0].normalDirection, Vector3.forward);
                                    DirectionalConstraint cn = n.gameObject.AddComponent(typeof(DirectionalConstraint)) as DirectionalConstraint;
                                    n.constraints.Add(cn);
                                    cn.normalDirection = otherDir;

                                    break;
                                case 2:
                                    sp.sprite = circle;
                                    foreach (DirectionalConstraint d in n.constraints)
                                    {
                                        Destroy(d);
                                    }
                                    n.constraints = new List<DirectionalConstraint>();
                                    break;
                            }
                        }
                    }
                    setMessage();
                }
            }
            else
            {
                if (hoveringBeam != null)
                {

                    positionHoveringBeam(selectPos3D);
                }

                //attempt to place second node and finish beam
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Node secondNode = createOrSelectNode(spos);
                    bool alreadyConnected = false;
                    foreach (Beam b in firstNode.beams)
                    {
                        if (b.nodes[0] == secondNode || b.nodes[1] == secondNode) { alreadyConnected = true; break; }
                    }
                    //place second node and finish beam
                    if (!alreadyConnected)
                    {

                        Beam newBeam = Instantiate(beamPrefab);
                        newBeam.transform.position = (firstNode.transform.position + secondNode.transform.position) / 2;
                        SpriteRenderer sp = newBeam.GetComponent<SpriteRenderer>();
                        sp.size = new Vector2((secondNode.transform.position - firstNode.transform.position).magnitude, 1f);
                        sp.transform.right = (secondNode.transform.position - firstNode.transform.position).normalized;
                        newBeam.nodes = new Node[] { firstNode, secondNode };
                        firstNode.beams.Add(newBeam);
                        secondNode.beams.Add(newBeam);
                        beams.Add(newBeam);

                        firstNode = null;
                        Destroy(hoveringBeam.gameObject);
                        setMessage();
                    }
                }
            }
        }
        else {
            //apply force;
            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                firstNode = selectNode(mousePos);
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                resetVisualization();
                firstNode = null;
            }

            if (firstNode != null) {
                Vector2 force = mousePos - new Vector2(firstNode.transform.position.x, firstNode.transform.position.y);
                LSE system = new LSE(nodes, beams, getConstraints());
                system.solveForce(new GradientDescent(), firstNode.index, force);

            }
            
        }

        
    }

    List<DirectionalConstraint> getConstraints() {
        List<DirectionalConstraint> ret = new List<DirectionalConstraint>();
        foreach (Node n in nodes) {
            foreach (DirectionalConstraint c in n.constraints)
                ret.Add(c);
        }
        return ret;
    }

    Vector2 selectPos(Vector2 pos) {

        Node select = selectNode(pos);
        if (select != null) return select.transform.position;
        //angle snapping;
        /*
        if (Input.GetKey(KeyCode.LeftShift)) {
            if (firstNode == null) return pos;
            Vector2 dirO = (new Vector3(pos.x, pos.y, 0) - firstNode.transform.position);
            Vector2 dir = angleSnapping(dirO);
            return new Vector2(firstNode.transform.position.x, firstNode.transform.position.y) + dir;
        }
        */
        //grid snapping
        if (grid) {
            float xPos = Mathf.Round(pos.x / gridSize) * gridSize;
            //Debug.Log(Mathf.Round(pos.x / gridSize) + " " + xPos + " " + );
            if (Mathf.Abs(xPos - pos.x) < snapRadius) pos.x = xPos;
            float yPos = Mathf.Round(pos.y / gridSize) * gridSize;
            if (Mathf.Abs(yPos - pos.y) < snapRadius) pos.y = yPos;
        }
        return pos;
    }

    Node selectNode(Vector2 pos) {
        Node ret = null;
        float closest = Mathf.Infinity;
        foreach (Node n in nodes)
        {
            float dist = (new Vector2(n.transform.position.x, n.transform.position.y) - pos).magnitude;
            if (dist < closest && dist < snapRadius) {
                ret = n;
                closest = dist;
            }
        }
        return ret;
    }

    Node createOrSelectNode(Vector2 pos) {
        Node ret = selectNode(pos);
        if (ret == null)
        {
            ret = Instantiate(nodePrefab);
            ret.transform.position = selectPos(pos);
            nodes.Add(ret);
        }
        return ret;
    }

    Vector2 angleSnapping(Vector2 v) {
        float angle = Mathf.Atan2(v.y, v.x);
        if (angle < 0) angle = 2 * Mathf.PI + angle;
        //Debug.Log(angle);

        float closest = 0;
        float closestDiff = Mathf.Infinity;

        //iterate 45 degree increments
        for (int i = 0; i < 8; i++) {
            float diff = Mathf.Abs(angle - i * Mathf.PI / 4);
            if (diff < closestDiff) {
                closest = i * Mathf.PI / 4;
                closestDiff = diff;
            }
        }

        //iterate 30 degree increments
        for (int i = 0; i < 12; i++)
        {
            float diff = Mathf.Abs(angle - i * Mathf.PI / 6);
            if (diff < closestDiff)
            {
                closest = i * Mathf.PI / 6;
                closestDiff = diff;
            }
        }

        return new Vector2(Mathf.Cos(closest), Mathf.Sin(closest)) * v.magnitude;
    }

    void positionHoveringBeam(Vector3 selectPos3D) {
        hoveringBeam.transform.position = (firstNode.transform.position + selectPos3D) / 2;
        SpriteRenderer sp = hoveringBeam.GetComponent<SpriteRenderer>();
        sp.size = new Vector2((selectPos3D - firstNode.transform.position).magnitude, 1f);
        sp.transform.right = (selectPos3D - firstNode.transform.position).normalized;
    }

    void resetVisualization() {
        foreach (Beam b in beams) {
            SpriteRenderer sp = b.GetComponent<SpriteRenderer>();
            sp.color = Color.white;
        }
    }

    public void switchMode() {
        if (construct)
        {
            if (activeConstraint != null) activeConstraint = null;
            if (hoveringNode != null) Destroy(hoveringNode.gameObject);
            if (hoveringBeam != null)
            {
                Destroy(hoveringBeam.gameObject);
                if (firstNode.beams.Count == 0)
                {
                    nodes.Remove(firstNode);
                    Destroy(firstNode.gameObject);
                }
            }
        }
        else
        {
            firstNode = null;
        }
        construct = !construct;
        setMessage();
    }

    void setMessage() {
        string t = "";
        string w = "";
        if (nodes.Count * 2 > getConstraints().Count + beams.Count) w = " WARNING: underconstrained, simulation will be incorrect";
        if (nodes.Count * 2 < getConstraints().Count + beams.Count) w = " WARNING: overconstrained, simulation will be incorrect";
        warning.text = w;
        if (construct) t = "Switch to Simulation Mode";
        else t = "Switch to Construction Mode";
        status.text = t;
    }

    void setHint() {
        string h = "";
        if (construct)
        {
            if (firstNode == null)
            {
                if (nodes.Count == 0) h = "left click to place a node and start drawing a beam";
                else h = "left click to place or select a node and start drawing a beam\nright click a node to add a constraint";
            }
            else {
                h = "left click to finish the beam";
            }
            if (activeConstraint != null) {
                h = "right click to finish orienting the constraint";
            }
        }
        else {
            h = "click and drag a node to apply a force";
        }
        hint.text = h;
    }

    public void deleteAll() {
        foreach (Beam b in beams) Destroy(b.gameObject);
        foreach (Node n in nodes) Destroy(n.gameObject);
        beams = new List<Beam>();
        nodes = new List<Node>();
    }

    
}

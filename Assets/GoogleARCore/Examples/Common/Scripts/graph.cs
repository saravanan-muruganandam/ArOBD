
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;


public class graph : MonoBehaviour {


	//Box collider to give shape to the gaph, and position
	private BoxCollider graphCollider;
	public Object GraphVerticalLinePrefab;
	// XY scale maximum minimum values
	public float plotValue = 100.0f;

	public float timeLinelengthInSec = 40f;
	public float initialMaxYscaleValue = 1500f;
	public float minYscaleValue = 0.0f;

	private float maxYscaleValue;
	private static System.DateTime currentTime ; 
	private static System.DateTime XscaleLastTime;

	//No of dividers in both X and Y scale
	public int noOfYpartition = 10;
	public float noOfXpartition = 10;

	// Game objects, X Y and graph line
	private Transform xAxisGameObject=null;
	private Transform yAxisGameObject=null;
	private Transform graphLineObject=null;

	//Line rendereer for X Y and graph lines
	private LineRenderer xLine=null;
	private LineRenderer yLine=null;

	private LineRenderer graphLine=null;
	MeshFilter meshFilter;
	GameObject filledGraph;
	MeshRenderer renderer;
	private Mesh graphLineMesh;
	Mesh filledGraphMesh;
	// Graph plot value
	private List<GraphPoint> graphPoints;
	private List<Vector3> graphLinePoints;
	public Object xDividerPrefab;
	public Object yDividerPrefab;
	[SerializeField]
	private Transform readingValueGameObject;
	private Transform titleNameObject;
	private Transform unitTextObject;
	// Four vertor point of a face in square
	private Vector3 vertice1;
	private Vector3 vertice2;
	private Vector3 vertice3;
	private Vector3 vertice4;
	//List of divider in the x and y scale lines 
	ArrayList yDividerGameobjects;
	ArrayList xDividerGameobjects;

	public float getXYslot(float axisValue, float axisMaxValue, float axisMinValue){
		float valuePosition=0.0f;
		if (axisValue < axisMaxValue) {
			valuePosition = (axisMaxValue-axisValue) / (axisMaxValue-axisMinValue);
		}
		return valuePosition;
	}

	public float getTimeDiffinPercentage(System.DateTime valueTime)
	{
		
		return (float)(currentTime - valueTime).TotalSeconds/ timeLinelengthInSec;
	}

	public System.DateTime getCurrentTime()
	{
		return System.DateTime.Now;
	}

	void Awake()
	{
		//readingValueGameObject = transform.Find("Top Bar/valueText");
		titleNameObject = transform.Find("Top Bar/Title");
		unitTextObject = transform.Find("Top Bar/Unittext");
		graphPoints = new List<GraphPoint>();
	}

	void OnEnable()
	{
		//Debug.Log("******* TOTAL NUMBER OF TAGED OBJECT IS ******" + new List<GameObject>(GameObject.FindGameObjectsWithTag(readingValueGameObject.tag)).Count);


		
		StartCoroutine("AddCurrentValue");

	}

	void Start () {

		maxYscaleValue = initialMaxYscaleValue;
		currentTime = getCurrentTime();
		//xDividerPrefab = AssetDatabase.LoadAssetAtPath("Assets/X_Div.prefab", typeof(GameObject));
		//yDividerPrefab = AssetDatabase.LoadAssetAtPath("Assets/Y_Div.prefab", typeof(GameObject));

		yDividerGameobjects= new ArrayList();
		xDividerGameobjects = new ArrayList();
		graphCollider = gameObject.GetComponent<BoxCollider>();

		graphLinePoints = new List<Vector3>();
		//XScaleline and YscaleLIne
		xAxisGameObject = transform.Find("xAxis");
		yAxisGameObject = transform.Find("yAxis");
		graphLineObject = transform.Find("GraphLine");
		graphLineObject.gameObject.SetActive (true);
		
		xLine = xAxisGameObject.GetComponent<LineRenderer>();
		yLine = yAxisGameObject.GetComponent<LineRenderer>();

		graphLine = graphLineObject.GetComponent<LineRenderer>();
		filledGraph = new GameObject("Filled graph");
		filledGraph.transform.parent = transform;
		
		renderer = filledGraph.AddComponent<MeshRenderer>();
		graphLineMesh = new Mesh();
		meshFilter = filledGraph.AddComponent<MeshFilter>();
		meshFilter.mesh = graphLineMesh;
		graphLineMesh.MarkDynamic();
		renderer.material = graphLine.material;


		xAxisGameObject.gameObject.SetActive(true);
		yAxisGameObject.gameObject.SetActive(true);
		graphLineObject.gameObject.SetActive(true);
		graphLine.loop = false;

		for (int i = 0; i < noOfYpartition + 1; i++)
		{
			GameObject yDivider = Instantiate(yDividerPrefab, yAxisGameObject) as GameObject;
			//yDivider.transform.localScale = new Vector3(0.1510651f, 0.1510651f, 0.1510651f);
			yDividerGameobjects.Add(yDivider);

		}
		for (int i = 0; i < noOfXpartition + 1; i++)
		{
			GameObject xDivider = Instantiate(xDividerPrefab, xAxisGameObject) as GameObject;
			//xDivider.transform.localScale = new Vector3(0.1510651f, 0.1510651f, 0.1510651f);
			xDividerGameobjects.Add(xDivider);

		}

	}

	// Update is called once per frame
	void Update()
	{
		currentTime = getCurrentTime();
		// four vector position of the rectangle face in a collider
		vertice1 = gameObject.transform.TransformPoint(graphCollider.center + new Vector3(-graphCollider.size.x, -graphCollider.size.y, -graphCollider.size.z)*0.5f);
		vertice2 = gameObject.transform.TransformPoint(graphCollider.center + new Vector3(graphCollider.size.x, -graphCollider.size.y, -graphCollider.size.z)*0.5f);
		vertice3 = gameObject.transform.TransformPoint(graphCollider.center + new Vector3(-graphCollider.size.x, graphCollider.size.y, -graphCollider.size.z)*0.5f);
		vertice4 = gameObject.transform.TransformPoint(graphCollider.center + new Vector3(graphCollider.size.x, graphCollider.size.y, -graphCollider.size.z) * 0.5f);

		//Debug.DrawLine(vertice3, vertice4, Color.blue);
		//line for x axis
		xLine.SetPosition(0, transform.InverseTransformPoint(vertice1));
		xLine.SetPosition(1, transform.InverseTransformPoint(vertice2));

		//line y scale end points
		yLine.SetPosition(0, transform.InverseTransformPoint(vertice1));
		yLine.SetPosition(1, transform.InverseTransformPoint(vertice3));

		// Updating reading value on Title 
		//titleBarReadingGameObject.GetComponent<TextMesh>().text = plotValue.ToString();
		foreach (GameObject divY in yDividerGameobjects)
		{
			divY.transform.position = Vector3.Lerp (vertice1, vertice3,(float) yDividerGameobjects.IndexOf(divY)/noOfYpartition);
			divY.transform.Find ("valueText").GetComponent<TextMeshPro>().text =  ((((float)yDividerGameobjects.IndexOf(divY)/noOfYpartition))*maxYscaleValue).ToString();
		}

		foreach (GameObject divX in xDividerGameobjects)
		{
			divX.transform.position = Vector3.Lerp (vertice1, vertice2,(float) xDividerGameobjects.IndexOf(divX)/noOfXpartition);

		}

		//GenerateMeshFromGraphValue(graphPoints);


	}

	void FixedUpdate()
	{
		
		XscaleLastTime = getCurrentTime().AddSeconds(-timeLinelengthInSec);

		foreach (GameObject divX in xDividerGameobjects)
		{
			divX.transform.Find("valueText").GetComponent<TextMeshPro>().text = (getCurrentTime().AddSeconds(-(double)(((xDividerGameobjects.IndexOf(divX) / noOfXpartition)) * (timeLinelengthInSec)))).ToString("HH:mm:ss");

		}
		graphLinePoints.Clear();
		graphPoints.FindAll(s => getTimeDiffinPercentage(s.ValueRecordedTime) >= 1f).ForEach(s => Destroy(s.MeshLineObject));
		graphPoints.RemoveAll (s => getTimeDiffinPercentage (s.ValueRecordedTime)>=1f);

		float maxValueInCurrentGraph = float.MinValue;
		
		
		foreach (GraphPoint graphPoint in graphPoints)
		{
			if ((getTimeDiffinPercentage(graphPoint.ValueRecordedTime)) >= 1f)
			{
				
				graphPoints.Remove(graphPoint);
				continue;
			}
			// Find the maximum graph point value
			if (graphPoint.GraphValue > maxValueInCurrentGraph)
			{
				maxValueInCurrentGraph = graphPoint.GraphValue;
			}

			float YLearpPercent = 1 - getXYslot(graphPoint.GraphValue, maxYscaleValue, minYscaleValue);
			Vector3 xScaleBelowPoint = Vector3.Lerp(vertice1, vertice2, getTimeDiffinPercentage(graphPoint.ValueRecordedTime)); 

			//modify this to adjust the graphpoint horizontal alignment and movement
			Vector3 xScaleTopPoint = Vector3.Lerp(vertice3, vertice4, getTimeDiffinPercentage(graphPoint.ValueRecordedTime));
			Vector3 actualPoint = Vector3.Lerp(xScaleBelowPoint, xScaleTopPoint, YLearpPercent);
			graphPoint.XlineBelowPoint = xScaleBelowPoint;
			graphPoint.WorldPositionPoint = actualPoint;
			if (graphPoint.MeshLineObject != null) {
				
				graphPoint.MeshLineObject.GetComponent<LineRenderer>().startWidth=graphCollider.size.x* 0.018f;
				graphPoint.MeshLineObject.GetComponent<LineRenderer>().endWidth = graphCollider.size.x * 0.018f;
				graphPoint.MeshLineObject.GetComponent<LineRenderer>().SetPosition(0, transform.InverseTransformPoint(graphPoint.XlineBelowPoint));
				graphPoint.MeshLineObject.GetComponent<LineRenderer>().SetPosition(1, transform.InverseTransformPoint(graphPoint.WorldPositionPoint));
				
			}
			graphLinePoints.Insert(graphPoints.IndexOf(graphPoint), transform.InverseTransformPoint(graphPoint.WorldPositionPoint));
		}
		

		//seting the max y scale limit, based on maximum graph point value;
		if (maxValueInCurrentGraph > initialMaxYscaleValue)
		{
			maxYscaleValue = maxValueInCurrentGraph;
		}
		else
		{
			maxYscaleValue = initialMaxYscaleValue;
		}


		graphLine.positionCount = graphLinePoints.Count;
		graphLine.SetPositions (graphLinePoints.ToArray());


	}

	private void GenerateMeshFromGraphValue(List<GraphPoint> GraphValuePoints)
	{

		List<Vector3> graphMeshVerticess = new List<Vector3>();
		int numTriangles = (GraphValuePoints.Count - 1) * 2;
		List<int> trianglesVertices = new List<int>();
		foreach (GraphPoint graphPoint in GraphValuePoints)
		{

			if (GraphValuePoints.IndexOf(graphPoint) >= 1)
			{
				List<Vector3> PlaneMeshVerticess = new List<Vector3>();
				int[] planetrianglesVertices = new int[6];
				PlaneMeshVerticess.Add(transform.InverseTransformPoint(GraphValuePoints[GraphValuePoints.IndexOf(graphPoint) - 1].XlineBelowPoint));
				PlaneMeshVerticess.Add(transform.InverseTransformPoint(GraphValuePoints[GraphValuePoints.IndexOf(graphPoint) - 1].WorldPositionPoint));
				PlaneMeshVerticess.Add(transform.InverseTransformPoint(graphPoint.WorldPositionPoint));
				PlaneMeshVerticess.Add(transform.InverseTransformPoint(graphPoint.XlineBelowPoint));

				graphMeshVerticess.AddRange(PlaneMeshVerticess);

				planetrianglesVertices[0] = graphMeshVerticess.IndexOf(PlaneMeshVerticess[0]);
				planetrianglesVertices[1] = graphMeshVerticess.IndexOf(PlaneMeshVerticess[1]);
				planetrianglesVertices[2] = graphMeshVerticess.IndexOf(PlaneMeshVerticess[3]);

				planetrianglesVertices[3] = graphMeshVerticess.IndexOf(PlaneMeshVerticess[1]);
				planetrianglesVertices[4] = graphMeshVerticess.IndexOf(PlaneMeshVerticess[2]);
				planetrianglesVertices[5] = graphMeshVerticess.IndexOf(PlaneMeshVerticess[3]);

				trianglesVertices.AddRange(planetrianglesVertices);

			}
		}
		Vector2[] uvs = new Vector2[graphMeshVerticess.Count];
		for (int i = 0; i < uvs.Length; i++)
		{
			uvs[i] = new Vector2(graphMeshVerticess[i].x, graphMeshVerticess[i].z);
		}
		graphLineMesh.vertices = graphMeshVerticess.ToArray();
		graphLineMesh.triangles = trianglesVertices.ToArray();
		graphLineMesh.uv = uvs;
		filledGraph.transform.position = transform.position;
		filledGraph.transform.localScale = Vector3.one;
	}



	void OnDisable()
	{
		StopCoroutine("AddCurrentValue");
		//Debug.Log("******* TOTAL NUMBER OF TAGED OBJECT IS ******"+ new List<GameObject>(GameObject.FindGameObjectsWithTag(readingValueGameObject.tag)).Count);
	}

	public void UpdateValueObjectTag(string TagName)
	{
		readingValueGameObject.tag = TagName;
	}

	public void UpdateTitleName(string TitleName)
	{
		titleNameObject.GetComponent<TextMeshPro>().text = TitleName;
	}

	public void UpdateUnitValue(string unitValue)
	{
		unitTextObject.GetComponent<TextMeshPro>().text = unitValue;
	}
	IEnumerator AddCurrentValue()
	{
		while (true)
		{
			System.Random random = new System.Random();
			string currentReading = readingValueGameObject.GetComponent<TextMeshPro>().text;
			if (!currentReading.Equals("NO DATA"))
			{
				GraphPoint graphpoint = new GraphPoint();
				float YValue = plotValue = (float)System.Convert.ToDouble(currentReading);//Random.Range(minYscaleValue, maxYscaleValue);
				graphpoint.GraphValue = YValue;
				graphpoint.ValueRecordedTime = getCurrentTime();
				if (GraphVerticalLinePrefab != null)
				{
					graphpoint.MeshLineObject = Instantiate(GraphVerticalLinePrefab, gameObject.transform) as GameObject;
					graphpoint.MeshLineObject.transform.position = transform.position;
					graphpoint.MeshLineObject.transform.localScale = Vector3.one;
				}

				graphPoints.Add(graphpoint);
				graphPoints.Sort(new sortGraphPointByTime());
			}
			
			yield return new WaitForSeconds(0.3f);

		}
	}

	public class sortGraphPointByTime : IComparer<GraphPoint>
	{

		public int Compare(GraphPoint a, GraphPoint b)
		{
			System.DateTime c1 = a.ValueRecordedTime;
			System.DateTime c2 = b.ValueRecordedTime;
			if (c1 > c2)
				return 1;
			return c1 < c2 ? -1 : 0;
		}
	}

	
}

public class GraphPoint{

	public System.DateTime ValueRecordedTime { get; set; }
	public float GraphValue{ get; set; }
	public Vector3 WorldPositionPoint{ get; set; } 
	public Vector3 XlineBelowPoint { get; set; }
	public GameObject MeshLineObject { get; set; }


}



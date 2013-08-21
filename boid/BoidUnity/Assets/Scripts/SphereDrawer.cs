using UnityEngine;
using System.Collections;
 
public class SphereDrawer : MonoBehaviour {

  public int xCount = 30;
  public int yCount = 30;
  
  void Start () 
  {
    radius = FlockManager.GetMain().kSphereRadius;
    CreateUVSphere();
  }

  void Update () 
  {

  }

  void CreateLineMaterial() 
  {
      if( !lineMaterial ) {
          lineMaterial = new Material( "Shader \"Lines/Colored Blended\" {" +
              "SubShader { Pass { " +
              "    Blend SrcAlpha OneMinusSrcAlpha " +
              "    ZWrite Off Cull Off Fog { Mode Off } " +
              "    BindChannels {" +
              "      Bind \"vertex\", vertex Bind \"color\", color }" +
              "} } }" );
          lineMaterial.hideFlags = HideFlags.HideAndDontSave;
          lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;}
  }

  void OnPostRender() 
  {   
      Vector3 forward = Camera.main.transform.forward;
      CreateLineMaterial();
      // set the current material
      lineMaterial.SetPass( 0 );

      GL.Begin( GL.LINES );

      GL.Color(mainColor);

      for(int i = 0; i < quadArray.Length; i++) {
        Quad q = quadArray[i];
        if(Vector3.Dot(q.normal, forward) > 0) {
          Vector3 vTl = verticesArray[q.tl];
          Vector3 vTr = verticesArray[q.tr];
          Vector3 vBr = verticesArray[q.br];
          Vector3 vBl = verticesArray[q.bl];

          // top
          GL.Vertex3( vTl.x, vTl.y, vTl.z );
          GL.Vertex3( vTr.x, vTr.y, vTr.z );
          // bottom
          GL.Vertex3( vBl.x, vBl.y, vBl.z );
          GL.Vertex3( vBr.x, vBr.y, vBr.z );
          // left
          GL.Vertex3( vTl.x, vTl.y, vTl.z );
          GL.Vertex3( vBl.x, vBl.y, vBl.z );
          // right
          GL.Vertex3( vTr.x, vTr.y, vTr.z );
          GL.Vertex3( vBr.x, vBr.y, vBr.z );
        }
      }

      GL.End();
  }

  private void CreateUVSphere() {
    verticesArray = new Vector3[xCount * yCount];
    quadArray = new Quad[(xCount - 1) * (yCount - 1)];

    // create vertices
    float deltaTeta = (Mathf.PI * 2.0f) / (float)(xCount - 1);
    float deltaPhi = Mathf.PI / (float)(yCount - 1);
    for(int y = 0; y < yCount; y++) {
      for(int x = 0; x < xCount; x++) {
        Vector3 v = new Vector3();
        float teta = deltaTeta * (float)x;
        float phi = - Mathf.PI * 0.5f + deltaPhi  * (float)y;
        v.x = Mathf.Cos(teta) * Mathf.Cos(phi) * radius;
        v.y = Mathf.Sin(phi) * radius;
        v.z = Mathf.Sin(teta) * Mathf.Cos(phi) * radius;
        verticesArray[x + y * xCount] = v;
      }
    }

    // create quads
    for(int y = 0; y < yCount - 1; y++) {
      for(int x = 0; x < xCount - 1; x++) {
        Quad q = new Quad();
        q.tl = x       + y       * xCount;
        q.tr = (x + 1) + y       * xCount;
        q.br = (x + 1) + (y + 1) * xCount;
        q.bl = x       + (y + 1) * xCount;
        q.normal = (verticesArray[q.tl] +
                    verticesArray[q.tr] +
                    verticesArray[q.br] +
                    verticesArray[q.bl]).normalized;
        quadArray[x + y * (xCount - 1)] = q;
      }
    }
  }

  private class Quad {
    public int tl; // top left
    public int tr; // top right
    public int br; // bottom right
    public int bl; // bottom left
    public Vector3 normal;
  }

  private Material lineMaterial;
  private Color mainColor = new Color(0f,1f,0f,1f);
  private Vector3 [] verticesArray;
  private Quad [] quadArray;
  private float radius = 200.0f;
}
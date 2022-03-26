using UnityEngine;
using System.Collections.Generic;
namespace SKCell
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer))]
    [AddComponentMenu("SKCell/SKModelProcessing")]
    public class SKModelProcessing : PostEffectsBase
    {
        #region Properties
        [Header("General Settings")]
        [Tooltip("Do not enable this unless you want this to update dynamically.")]
        public bool updateOnPlay = false;

        [Header("Color Properties")]
        [Range(0, 1)]
        public float saturation = 1;
        [Range(0, 1)]
        public float brightness = 0;
        public Color color = Color.white;

        [Header("Outline and Rim Light")]
        public Color rimColor = Color.white;
        [Range(0, 2)]
        public float rimWidth= 1f;

        #endregion

        #region References
        private MeshRenderer mr;
        private Shader alphaShader;
        private Material _materal;
        public Material _Material
        {
            get
            {
                _materal = CheckShaderAndCreateMaterial(alphaShader, _materal);
                return _materal;
            }
        }
        #endregion
        private void OnEnable()
        {
            alphaShader = Shader.Find("SKCell/ModelProcessing");
            mr = GetComponent<MeshRenderer>();

            mr.material = _Material;
            MeshFilter mf = GetComponent<MeshFilter>();
            if (mf)
            {
                //MeshNormalAverage(mf.sharedMesh);
            }
        }
        private void OnDisable()
        {
            mr.material = null;
        }
        void Update()
        {
            if (Application.isPlaying)
            {
                if (!updateOnPlay)
                {
                    return;
                }
            }
            _Material.SetFloat("_Saturation", saturation);
            _Material.SetFloat("_Brightness", brightness);
            _Material.SetFloat("_RimWidth", rimWidth);
            _Material.SetColor("_RimColor", rimColor);
            _Material.SetColor("_Color", color);
        }

        public void MeshNormalAverage(Mesh mesh)
        {
            Dictionary<Vector3, List<int>> map = new Dictionary<Vector3, List<int>>();

            for (int i = 0; i < mesh.vertexCount; i++)
            {
                if (!map.ContainsKey(mesh.vertices[i]))
                {
                    map.Add(mesh.vertices[i], new List<int>());
                }

                map[mesh.vertices[i]].Add(i);
            }

            Vector3[] normals = mesh.normals;
            Vector3 normal;

            foreach (var p in map)
            {
                normal = Vector3.zero;

                foreach (var n in p.Value)
                {
                    normal += mesh.normals[n];
                }

                normal /= p.Value.Count;

                foreach (var n in p.Value)
                {
                    normals[n] = normal;
                }
            }
            mesh.normals = normals;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace STYLY.MaintenanceTool.Utility
{
    /// <summary>
    /// STYLY Pluginの機能を実装するUtilityクラス
    /// </summary>
    public class STYLYPluginUtility
    {
        private const string OutputPath = "_Output/";

        public GameObject[] FindPrefabedGameObjectsInHierarchy(string scenePath)
        {
            List<GameObject> findGameObjects = new List<GameObject>();

            Scene scene = SceneManager.GetSceneByPath(scenePath);
            // 取得したシーンのルートにあるオブジェクトを取得
            GameObject[] rootObjects = scene.GetRootGameObjects();

            // 取得したオブジェクトの名前を表示
            foreach (GameObject gameObject in rootObjects)
            {
                findGameObjects.AddRange(GetPrefabedGameObject(gameObject));
            }

            return findGameObjects.ToArray();
        }

        List<GameObject> GetPrefabedGameObject(GameObject gameObject)
        {
            List<GameObject> gameObjects = new List<GameObject>();
            var prefab = PrefabUtility.GetPrefabParent(gameObject);//Prefabを取得
            // Prefab化されたGameObjectが見つかったら、それ以上は探索しない。
            if (prefab != null)
            {
                gameObjects.Add(gameObject);
                return gameObjects;
            }
            foreach (Transform child in gameObject.transform)
            {
                var goList = GetPrefabedGameObject(child.gameObject);
                if (goList != null)
                {
                    gameObjects.AddRange(goList);
                }
            }

            return gameObjects;
        }

        /// <summary>
        /// PrefabをビルドしてSTYLY Assetを作る
        /// </summary>
        /// <param name="prefab"></param>
        public bool BuildSTYLYAsset(GameObject prefab, BuildTarget buildTarget, string guid = null)
        {
            var abUtility = new AssetBundleUtility();

            if (guid == null)
            {
                guid = abUtility.GenerateGUID();
            }

            string outputPath = Path.Combine(OutputPath + "STYLY_ASSET", abUtility.GetPlatformName(buildTarget));

            var buildResult = abUtility.Build(guid, AssetDatabase.GetAssetPath(prefab), outputPath, buildTarget);

            return (buildResult != null);
        }

        /// <summary>
        /// AssetBundleのビルドを実行する
        /// </summary>
        /// <param name="scenePath">シーンのパス</param>
        /// <param name="buildTarget">ビルドターゲット（プラットフォーム）</param>
        /// <param name="guid">アセットバンドルのGUID</param>
        /// <returns></returns>
        public bool BuildSTYLYSceneAsset(string scenePath, BuildTarget buildTarget, string guid = null)
        {
            Debug.Log("BuildSTYLYSceneAsset:guid:" + guid);
            var abUtility = new AssetBundleUtility();

            // プラットフォーム切換え
            abUtility.SwitchPlatformAndPlayerSettings(buildTarget);

            if (guid == null)
            {
                guid = abUtility.GenerateGUID();
            }

            string outputPath = Path.Combine(OutputPath + "STYLY_ASSET", abUtility.GetPlatformName(buildTarget));

            var buildResult = abUtility.Build(guid, scenePath, outputPath, buildTarget);

            return (buildResult != null);
        }

        public string GenerateSceneXML(string scenePath)
        {
            GameObject[] gameObjects = FindPrefabedGameObjectsInHierarchy(scenePath);
            var result = CreateStylyAssetDataSet(gameObjects);
            return STYLY.STYLY_Functions.XmlUtil.SerializeToXmlString<stylyAssetDataSet>(result);
        }

        public stylyAssetDataSet CreateStylyAssetDataSet(GameObject[] stylyAssets)
        {
            //保存するアセットの情報をシリアライズするかするためにクラスに格納
            List<stylyAssetData> _stylyAssetsDataSetList = new List<stylyAssetData>();
            stylyAssetDataSet _stylyAssetsDataSet = new stylyAssetDataSet();

            //STYLY_Assets以下の子オブジェクト一覧を取得
            foreach (GameObject stylyAsset in stylyAssets)
            {
                //1つずつアセットの情報をクラスに格納
                stylyAssetData _stylyAssetsData = new stylyAssetData();
                _stylyAssetsData.prefabName = GetBuildedGUID(stylyAsset);
                _stylyAssetsData.Position = stylyAsset.transform.position;
                _stylyAssetsData.Rotation = stylyAsset.transform.rotation;
                _stylyAssetsData.Scale = stylyAsset.transform.localScale;
                _stylyAssetsData.title = stylyAsset.name;
                _stylyAssetsData.description = "";
                _stylyAssetsData.exclusiveCategory = "";
                _stylyAssetsData.itemURL = "";
                _stylyAssetsData.vals = new string[] { };
                _stylyAssetsData.visible = (stylyAsset.activeSelf) ? true.ToString() : false.ToString();

                //DataSetに追加
                _stylyAssetsDataSetList.Add(_stylyAssetsData);
                _stylyAssetsDataSet.AssetDataSet = _stylyAssetsDataSetList.ToArray();
            }

            return _stylyAssetsDataSet;
        }

        string GetBuildedGUID(GameObject gameObject)
        {
            var prefabPath = AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabParent(gameObject));
            var abUtility = new AssetBundleUtility();
            var buildedAssetData = abUtility.GetBuildedAssetData();
            var buildedData = buildedAssetData.GetData(prefabPath);
            string guid = null;
            buildedData.TryGetValue(BuildedAssetPathData.GUID_KEY, out guid);
            return guid;
        }

        public string GenerateSceneXMLforSceneOnly(string scenePath)
        {
            var stylyAssetDataSet = CreateStylyAssetDataSetforSceneOnly(scenePath);
            return STYLY.STYLY_Functions.XmlUtil.SerializeToXmlString<stylyAssetDataSet>(stylyAssetDataSet);
        }

        public stylyAssetDataSet CreateStylyAssetDataSetforSceneOnly(string scenePath)
        {
            //保存するアセットの情報をシリアライズするかするためにクラスに格納
            List<stylyAssetData> _stylyAssetsDataSetList = new List<stylyAssetData>();
            stylyAssetDataSet _stylyAssetsDataSet = new stylyAssetDataSet();

            stylyAssetData _stylyAssetsData = new stylyAssetData();
            _stylyAssetsData.prefabName = GetBuildedGUID(scenePath);
            _stylyAssetsData.Position = Vector3.zero;
            _stylyAssetsData.Rotation = Quaternion.identity;
            _stylyAssetsData.Scale = Vector3.one;
            _stylyAssetsData.title = Path.GetFileNameWithoutExtension(scenePath);
            _stylyAssetsData.description = "";
            _stylyAssetsData.exclusiveCategory = "scene";
            _stylyAssetsData.itemURL = "";
            _stylyAssetsData.vals = new string[] { };
            _stylyAssetsData.visible = true.ToString();

            //DataSetに追加
            _stylyAssetsDataSetList.Add(_stylyAssetsData);
            _stylyAssetsDataSet.AssetDataSet = _stylyAssetsDataSetList.ToArray();

            return _stylyAssetsDataSet;
        }

        public string GenerateSceneXMLPrefabAndScene(string prefaScenePath, string scenePath)
        {
            GameObject[] gameObjects = FindPrefabedGameObjectsInHierarchy(prefaScenePath);
            var stylyAssetDataSet = CreateStylyAssetDataSet(gameObjects);

            var stylyAssetDataSetSene = CreateStylyAssetDataSetforSceneOnly(scenePath);

            stylyAssetDataSet.AssetDataSet = stylyAssetDataSet.AssetDataSet.Concat(stylyAssetDataSetSene.AssetDataSet).ToArray();

            return STYLY.STYLY_Functions.XmlUtil.SerializeToXmlString<stylyAssetDataSet>(stylyAssetDataSet);
        }

        string GetBuildedGUID(string prefabPath)
        {
            var abUtility = new AssetBundleUtility();
            var buildedAssetData = abUtility.GetBuildedAssetData();
            var buildedData = buildedAssetData.GetData(prefabPath);
            string guid = null;
            buildedData.TryGetValue(BuildedAssetPathData.GUID_KEY, out guid);
            return guid;
        }

        Dictionary<string, string> GetBuildedData(GameObject gameObject)
        {
            var prefabPath = AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabParent(gameObject));
            var abUtility = new AssetBundleUtility();
            var buildedData = abUtility.GetBuildedAssetData();
            var index = buildedData.prefabPathList.IndexOf(prefabPath);

            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("prefab path", buildedData.prefabPathList[index]);
            result.Add("guid", buildedData.guidList[index]);

            return result;
        }

        private const string SCENE_XML_REPLACE_TARGET = "<SCENE XML>";

        public string GenerateSceneInfoJson(string sceneXml, string templatePath)
        {
            // xmlの手動エスケープ
            sceneXml = sceneXml.Replace("\"", "\\\"");
            sceneXml = sceneXml.Replace("/", "\\/");
            sceneXml = sceneXml.Replace(Environment.NewLine, "\\r\\n");

            var templateString = File.ReadAllText(templatePath);
            templateString = templateString.Replace(SCENE_XML_REPLACE_TARGET, sceneXml);
            return templateString;
        }

        public int CopyBuildedAssetBundleToSTYLY(GameObject[] gameObjects)
        {
            var abUtility = new AssetBundleUtility();
            var pcvrUtility = new PCVRUtility();
            var buildedPathData = abUtility.GetBuildedAssetData();
            var paltformName = "Windows";

            foreach (var gameObject in gameObjects)
            {
                var buildedInfo = GetBuildedData(gameObject);
                string srcPath = Path.Combine(OutputPath + "STYLY_ASSET", paltformName);
                srcPath = CommonUtility.FixPathString(Path.Combine(srcPath, buildedInfo["guid"]));
                Debug.Log("assetBundlePath:" + srcPath);

                string destPath = CommonUtility.FixPathString(Path.Combine(pcvrUtility.GetSTYLYPersistentDataPath(), "STYLY_ASSET\\" + paltformName));
                destPath = CommonUtility.FixPathString(Path.Combine(destPath, buildedInfo["guid"]));
                Debug.Log("STYLYPath:" + destPath);
                var result = pcvrUtility.PushFile(srcPath, destPath);

                if (result != 0)
                {
                    return -1;
                }
            }

            return 0;
        }

        public bool SaveNoPrefabScene(string scenePath, string destPath)
        {
            Scene scene = EditorSceneManager.OpenScene(scenePath);
            // var gameObjects = FindPrefabedGameObjectsInHierarchy(scenePath);
            List<GameObject> findGameObjects = new List<GameObject>();

            // 取得したシーンのルートにあるオブジェクトを取得
            GameObject[] rootObjects = scene.GetRootGameObjects();

            // 取得したオブジェクトの名前を表示
            foreach (GameObject gameObject in rootObjects)
            {
                findGameObjects.AddRange(GetPrefabedGameObject(gameObject));
            }

            foreach (var gameObject in findGameObjects)
            {
                GameObject.DestroyImmediate(gameObject);
            }

            var result = EditorSceneManager.SaveScene(scene, destPath, false);

            EditorSceneManager.OpenScene(scenePath);

            return result;
        }
    }
}

namespace STYLY.STYLY_Functions
{
    //http://ftvoid.com/blog/post/1061
    public class XmlUtil
    {
        // シリアライズしてファイルに保存（XMLはUTF-8のBOM無しで保存すること。そうしないと読み込み時にエラーが出ます。）
        public static T SeializeToFile<T>(string filename, T data)
        {
            using (var stream = new System.IO.StreamWriter(filename, false, new System.Text.UTF8Encoding(false)))
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stream, data);
            }
            return data;
        }

        // クラスのシリアライズをXMLテキストで取得
        public static string SerializeToXmlString<T>(T data)
        {
            var output = new MemoryStream();
            var settings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };

            using (var xmlWriter = XmlWriter.Create(output, settings))
            {
                var serializer = new XmlSerializer(typeof(T));
                var namespaces = new XmlSerializerNamespaces();
                xmlWriter.WriteStartDocument();
                namespaces.Add(string.Empty, string.Empty);
                serializer.Serialize(xmlWriter, data, namespaces);
            }
            output.Seek(0L, SeekOrigin.Begin);
            string xml = new StreamReader(output).ReadToEnd();
            return xml;
        }

        // XML文字列からデシリアライズ
        public static T DeserializeFromXmlString<T>(string XmlString)
        {
            Debug.Log(XmlString);
            var serializer = new XmlSerializer(typeof(T));
            try
            {
                return (T)serializer.Deserialize(new StringReader(XmlString));
            }
            catch (XmlException e)
            {
                Debug.LogError(e.StackTrace);
                var lastAssetIndex = XmlString.LastIndexOf("<stylyAssetData>");
                if (lastAssetIndex <= 0)
                {
                    TextAsset textAsset = Resources.Load("default") as TextAsset;
                    var recoverXML = textAsset.text;
                    return (T)serializer.Deserialize(new StringReader(recoverXML));
                }
                else
                {
                    var recoverXML = XmlString.Substring(0, lastAssetIndex);
                    recoverXML += "</AssetDataSet></stylyAssetDataSet>";
                    return (T)serializer.Deserialize(new StringReader(recoverXML));
                }
            }
        }
    }
}

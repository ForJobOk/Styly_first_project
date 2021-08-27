namespace STYLY.MaintenanceTool.Utility
{
    /// <summary>
    /// プラットフォームユーティリティーのインターフェース
    /// <para>派生クラス: PCVRUtility, AndroidVRUtility等</para>
    /// </summary>
    public interface IPlatformUtility
    {

        int StartSTYLY();

        int StartSTYLY(string urlScheme);
        int RunExternalProcessSync(string command, string[] argsArray, out string outputString, out string errorString);

        string PullFile(string path);

        int PushFile(string srcPath, string destPath);

        string CreateURLScheme(string guid, string userName = null, bool testMode = false);

        void SaveSceneJsonToSTYLY(string sceneInfoJson, string sceneGuid);

        int CopyBuildedAssetBundleToSTYLY(string guid);

        void ClearSTYLYTestMode();

        void SaveSceneXmlToSTYLYTestMode(string sceneXml);

        int CopyBuildedAssetBundleToSTYLYTestMode(string guid);
    }
}

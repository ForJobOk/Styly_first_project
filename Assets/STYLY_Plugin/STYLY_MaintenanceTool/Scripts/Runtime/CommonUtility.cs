using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace STYLY.MaintenanceTool.Utility
{

    /// <summary>
    /// 共通Utilityクラス
    /// </summary>
    public class CommonUtility
    {
        /// <summary>
        /// リトライの制限時間
        /// 長時間かかる処理はADBコマンドにはないはずなので、短い時間にする。問題があれば見直す。
        /// kill-serverコマンド実行時にタイムアウトすることがしばしばある。
        /// </summary>
        private static readonly TimeSpan timeout = TimeSpan.FromSeconds(10);

        /// <summary>リトライ間隔（ミリ秒）</summary>
        private static readonly int interval = 100;

        private Process process;
        private StringBuilder outputStringBuilder = null;
        private StringBuilder errorStringBuilder = null;

        public int RunExternalProcessSync(string command, string[] argsArray, out string outputString, out string errorString)
        {
            string args = string.Join(" ", argsArray);
            Debug.Log("command:" + command + " " + args);

            int exitCode = -1;
            var startInfo = new ProcessStartInfo(command, args);

            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = false;            // 入力を読み取り不可.
            startInfo.CreateNoWindow = true;

            outputStringBuilder = new StringBuilder("");
            errorStringBuilder = new StringBuilder("");

            process = Process.Start(startInfo);
            process.OutputDataReceived += new DataReceivedEventHandler(OutputDataReceivedHandler);
            process.ErrorDataReceived += new DataReceivedEventHandler(ErrorDataReceivedHandler);

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            
            try
            {
                var maxRetryNum = (int)(timeout.TotalMilliseconds / interval);
                var retryCount = 0;
                
                while (!process.WaitForExit(interval))
                {
                    Debug.Log("Waiting...");
                
                    // 制限時間を超えたら子プロセスを終了させる。（無限リトライさせない）
                    if (retryCount++ >= maxRetryNum)
                    {
                        process.Kill();
                        throw new TimeoutException("Timeout: " + command + " " + args);
                    }
                }

                // Timeout時はExitCodeはないので、ここを通ってはならない
                exitCode = process.ExitCode;
            }
            catch (Exception e)
            {
                Debug.LogWarningFormat("exception {0}", e.Message);
            }
            
            process.Close();

            outputString = outputStringBuilder.ToString();
            errorString = errorStringBuilder.ToString();

            outputStringBuilder = null;
            errorStringBuilder = null;

            return exitCode;
        }

        private void OutputDataReceivedHandler(object sendingProcess, DataReceivedEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Data))
            {
                outputStringBuilder.Append(args.Data);
                outputStringBuilder.Append(Environment.NewLine);
            }
        }

        private void ErrorDataReceivedHandler(object sendingProcess, DataReceivedEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Data))
            {
                errorStringBuilder.Append(args.Data);
                errorStringBuilder.Append(Environment.NewLine);
            }
        }


        public static string FixPathString(string srcPath)
        {
            srcPath = srcPath.Replace('/', Path.DirectorySeparatorChar);
            srcPath = srcPath.Replace('\\', Path.DirectorySeparatorChar);

            return srcPath;
        }

        public string GetPathInHierarchy(GameObject gameObject)
        {
            string path = "";

            path = GetPath(gameObject.transform, path);

            return path;
        }

        string GetPath(Transform tr, string path)
        {
            var parent = tr.parent;

            if (parent != null)
            {
                path = GetPath(parent, path) + "-" + TransformToIndex(tr);
            }
            else
            {
                path = TransformToIndex(tr).ToString();
            }
            return path;
        }

        int TransformToIndex(Transform tr)
        {
            var index = 0;

            if (tr.parent != null)
            {
                foreach (Transform ch in tr.parent.transform)
                {
                    if (tr == ch)
                    {
                        break;
                    }
                    index++;
                }
            }
            else
            {
                var rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects().ToList();

                foreach (GameObject go in rootGameObjects)
                {
                    if (tr == go.transform)
                    {
                        break;
                    }
                    index++;
                }
            }

            return index;
        }


        public GameObject GetGameObjectByHierarchyPath(string path)
        {
            var pathIndexList = path.Split('-').ToList();

            var rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects().ToList();

            if (pathIndexList.Count == 0)
            {
                return null;
            }

            GameObject targetGo = rootGameObjects[int.Parse(pathIndexList[0])];

            if (pathIndexList.Count > 1)
            {
                return GetGameObjectByIndex(targetGo.transform, pathIndexList.GetRange(1, pathIndexList.Count - 1)).gameObject;
            }
            else
            {
                return targetGo;
            }
        }

        public Transform GetGameObjectByIndex(Transform target, List<string> indexList)
        {
            foreach (var indexStr in indexList)
            {
                var index = int.Parse(indexStr);
                target = target.transform.GetChild(index);
            }

            return target;
        }

    }
}
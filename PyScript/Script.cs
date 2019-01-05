using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Autodesk.DesignScript.Runtime;
using Dynamo.Applications;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace PyScript
{
    public class Script
    {
        private Script() { }

        /// <summary>
        /// Running a python script with the same name as the workspace.
        /// </summary>
        /// <returns>The script output. </returns>
        /// <search>python,script,code</search>
        [MultiReturn(new[] { "output" })]
        public static Dictionary<string, string> Execute()
        {
            var output = new Dictionary<string, string>
            {
                {"output", "" }
            };

            // Workspace file
            var model = DynamoRevit.RevitDynamoModel;
            Dynamo.Graph.Workspaces.WorkspaceModel ws = model.CurrentWorkspace;
            string workspacePath = ws.FileName;
            if (workspacePath == "")
            {
                output["output"] = "File has not been saved yet.";
                return output;
            }

            // Script file
            string dir = Path.GetDirectoryName(workspacePath);
            string filename = Path.GetFileNameWithoutExtension(workspacePath);
            string scriptPath = Path.Combine(dir, filename);
            scriptPath += ".py";
            if (!File.Exists(scriptPath))
            {
                output["output"] = "Script file does not exists.";
                return output;
            }

            ScriptEngine engine = Python.CreateEngine();
            ScriptScope scope = engine.CreateScope();

            // Search paths
            var paths = engine.GetSearchPaths();
            paths.Add(dir);
            engine.SetSearchPaths(paths);

            // Output
            var streamOut = new MemoryStream();
            engine.Runtime.IO.SetOutput(streamOut, Encoding.Default);
            engine.Runtime.IO.SetErrorOutput(streamOut, Encoding.Default);

            try
            {
                engine.ExecuteFile(scriptPath, scope);
            }
            catch (Exception e)
            {
                output["output"] = e.Message.ToString() + e.StackTrace.ToString();
                return output;
            }

            output["output"] = Encoding.Default.GetString(streamOut.ToArray());
            return output;
        }
    }
}

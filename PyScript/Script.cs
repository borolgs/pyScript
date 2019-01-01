using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Dynamo.Core;
using Dynamo.Applications;
using Autodesk.DesignScript.Runtime;
using RevitServices;

using IronPython.Runtime.Exceptions;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace PyScript
{
    public class Script
    {
        private Script() { }

        public static string Execute()
        {
            // Workspace file
            var model = DynamoRevit.RevitDynamoModel;
            Dynamo.Graph.Workspaces.WorkspaceModel ws = model.CurrentWorkspace;
            string workspacePath = ws.FileName;
            // string workspacePath = "E:/Users/olal/Desktop/test/PyScriptTest.dyn";
            if (workspacePath == "")
            {
                return "File has not been saved yet.";
            }

            // Script file
            string dir = Path.GetDirectoryName(workspacePath);
            string filename = Path.GetFileNameWithoutExtension(workspacePath);
            string scriptPath = Path.Combine(dir, filename);
            scriptPath += ".py";
            if (!File.Exists(scriptPath))
            {
                return "Script file does not exists.";
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
                return e.Message.ToString() + e.StackTrace.ToString();
            }

            return Encoding.Default.GetString(streamOut.ToArray());
        }
    }
}

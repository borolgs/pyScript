using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Autodesk.Revit.DB;
using Revit.Elements;
using Revit.GeometryConversion;

using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Dynamo.Applications;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Dynamo.Graph.Nodes;

namespace PyScript13
{
    public class Script
    {
        private Script() { }

        /// <summary>
        /// Running a python script with the same name as the workspace.
        /// </summary>
        /// <returns>The script output. </returns>
        /// <search>python,script,code</search>
        [MultiReturn(new[] { "output", "OUT" })]
        public static Dictionary<string, object> Execute()
        {
            var output = new Dictionary<string, object>
            {
                {"output", "" },
                {"OUT", null }
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
            string parentDir = Directory.GetParent(dir).FullName;
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

            // Variables
            scope.SetVariable("Convert", new Convert());
            scope.SetVariable("workspace_dir", dir);

            // Search paths
            var paths = engine.GetSearchPaths();
            paths.Add(dir);
            paths.Add(parentDir);
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
                string message = engine.GetService<ExceptionOperations>().FormatException(e);
                output["output"] = Encoding.Default.GetString(streamOut.ToArray()) + message;
                return output;
            }

            output["output"] = Encoding.Default.GetString(streamOut.ToArray());

            if (scope.ContainsVariable("OUT"))
            {
                output["OUT"] = scope.GetVariable("OUT");
            }

            return output;


        }

        [IsVisibleInDynamoLibrary(false)]
        public class Convert
        {

            public Autodesk.DesignScript.Geometry.Point ToPoint(XYZ p)
            {
                return p.ToPoint();
            }

            public Autodesk.DesignScript.Geometry.Solid ToSolid(Autodesk.Revit.DB.Solid e)
            {
                return e.ToProtoType();
            }

            public IEnumerable<Autodesk.DesignScript.Geometry.Surface> ToSurface(Autodesk.Revit.DB.Face e)
            {
                return e.ToProtoType();
            }
        }

    }
}

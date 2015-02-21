using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.ModelLoader
{
    public interface IModelLoader
    {
        string ClassName { get; }

        Models.GameObject CreateObject();

        /// <summary>Load and Object with data from an XML file</summary>
        /// <param name="root">Root element of the XML data</param>
        /// <param name="obj">Object to be filled</param>
        void LoadObject(Models.GameObject obj, GomObject gom);

        /// <summary>Load References to other objects</summary>
        /// <param name="root">Root XML Element</param>
        /// <param name="obj">Object to load</param>
        /// <remarks>This is done in a separate call to avoid problems with circular references and objects throwing exceptions</remarks>
        void LoadReferences(Models.GameObject obj, GomObject gom);
    }
}

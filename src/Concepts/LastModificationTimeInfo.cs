using Rhetos.Compiler;
using Rhetos.Dom.DefaultConcepts;
using Rhetos.Dsl;
using Rhetos.Dsl.DefaultConcepts;
using Rhetos.Extensibility;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concepts
{
    [Export(typeof(IConceptInfo))]
    [ConceptKeyword("LastModificationTime")]
    public class LastModificationTimeInfo : IConceptInfo
    {
        [ConceptKey]
        public DateTimePropertyInfo Property { get; set;  }
    }

    [Export(typeof(IConceptCodeGenerator))]
    [ExportMetadata(MefProvider.Implements, typeof(LastModificationTimeInfo))]
    public class LastModificationTimeCodeGenerator : IConceptCodeGenerator
    {
        public void GenerateCode(IConceptInfo conceptInfo, ICodeBuilder codeBuilder)
        {
            var info = (LastModificationTimeInfo)conceptInfo;

            string snippet =
            $@"{{ 
                foreach (var item in insertedNew.Concat(updatedNew))
                    item.{info.Property.Name} = DateTime.Now;
            }}
            ";

            codeBuilder.InsertCode(snippet,
                WritableOrmDataStructureCodeGenerator.InitializationTag,
                info.Property.DataStructure);
        }
    }
}

using Rhetos.Dsl;
using Rhetos.Dsl.DefaultConcepts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concepts
{
    [Export(typeof(IConceptInfo))]
    [ConceptKeyword("MonitoredRecord")]
    public class MonitoredRecordInfo: IConceptInfo
    {
        [ConceptKey]
        public EntityInfo Entity { get; set; }
    }

    public class MonitoredRecordMacro : IConceptMacro<MonitoredRecordInfo>
    {
        public IEnumerable<IConceptInfo> CreateNewConcepts(MonitoredRecordInfo conceptInfo, IDslModel existingConcepts)
        {
            var newConcepts = new List<IConceptInfo>();

            // The "MonitoredRecord”" concept will generate the following features:
            // DateTime CreatedAt { CreationTime; DenyUserEdit; }
            // Logging { AllProperties; }


            var createdAtProperty = new DateTimePropertyInfo
            {
                Name = "CreatedAt",
                DataStructure = conceptInfo.Entity
               
            };
            newConcepts.Add(createdAtProperty);
            
            // CreationTime;
            newConcepts.Add(new CreationTimeInfo
            {
                Property = createdAtProperty
            });

            // DenyUserEdit;
            newConcepts.Add(new DenyUserEditPropertyInfo
            {
                Property = createdAtProperty
            });

            return newConcepts;
        }
    }
}

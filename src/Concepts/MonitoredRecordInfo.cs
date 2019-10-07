using Rhetos.Dsl;
using Rhetos.Dsl.DefaultConcepts;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Concepts
{
    [Export(typeof(IConceptInfo))]
    [ConceptKeyword("MonitoredRecord")]
    public class MonitoredRecordInfo: IConceptInfo
    {
        [ConceptKey]
        public EntityInfo Entity { get; set; }
    }

    [Export(typeof(IConceptMacro))]
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

            // Logging
            var loggingInfo = new EntityLoggingInfo { Entity = conceptInfo.Entity };
            newConcepts.Add(loggingInfo);
            newConcepts.Add(new AllPropertiesLoggingInfo { EntityLogging = loggingInfo });

            return newConcepts;
        }
    }
}

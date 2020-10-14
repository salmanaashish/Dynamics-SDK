using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System.IO;
using System.Xml;
using Microsoft.Xrm.Sdk.Messages;

namespace DynamicsCrud
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string connectionString = ConfigurationManager.ConnectionStrings["CRM"].ConnectionString;
            CrmServiceClient conn = new CrmServiceClient(connectionString);
            IOrganizationService crmService = conn.OrganizationServiceProxy;
            try
            {
              
                //update a record

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        public void CreateRecord(IOrganizationService crmService)
        {
            //create a record
            Guid accountID = new Guid("E4163CB4-6869-E811-A957-000D3A******");

            // set targeted entity
            Entity allData = new Entity("sk_alldatatypes");

            // field type as string
            allData["sk_name"] = "Default Name";

            // field type as lookup
            allData["sk_account"] = new EntityReference("account", accountID);

            // field type as single line of text
            allData["sk_yourname"] = "Shaikh Sharef";

            // field type as bool/ two optionset
            allData["sk_gender"] = true;

            // field type as date
            allData["sk_dateofbirth"] = DateTime.Now; //DateTime.Parse(empDate);

            // field type as multiselect optionsets
            OptionSetValueCollection multiOptionSet = new OptionSetValueCollection();
            multiOptionSet.Add(new OptionSetValue(2)); //C#
            multiOptionSet.Add(new OptionSetValue(5)); //power bi
            allData["sk_selectyourcourses"] = new OptionSetValueCollection(multiOptionSet);

            // field type as optionset
            allData["sk_selectcountry"] = new OptionSetValue(2);

            // field type as number
            allData["sk_mobilenum"] = "8149497990";

            // field type as decimal
            allData["sk_empsalary"] = new Money(Convert.ToDecimal(25000.00));

            // field type as money/ integer
            allData["sk_salary"] = 230;

            // field type as float
            allData["sk_percentage"] = 72.77;

            // field type as multi select optionset
            allData["sk_address"] = "Mahedipatnam, Hyderabad \n  TS \n 50008 ";

            // execute
            Guid allDataCreate = crmService.Create(allData);

        }
        public void UpdateRecord(string entityName, Guid id, IOrganizationService CrmService)
        {
            Entity ent = new Entity()
            {
                LogicalName = entityName, // Entity logical name
                Id = id
            };
            ent["name"] = "ABC Corporation"; //single line text
            ent["accountcategorycode"] = new OptionSetValue(1); //option set
            ent["address1_longitude"] = 102.23232; //float number
            ent["openrevenue"] = new Money(100); // currency
            ent["new_establishedon"] = new DateTime(2010, 1, 1); //Date time
            ent["new_overflowrate"] = 4.5;//Decimal
            ent["parentaccountid"] = new EntityReference("account", new Guid("E491C599 - 9D0B - E611 - 80E7 - C4346BDD3141")); //Lookup type
            ent["description"] = "Test Test Test Test Test Test Test Test Test Test Test ";
            ent["creditonhold"] = false; //Two options
            ent["opendeals"] = 19;
            CrmService.Update(ent);

        }

        public Entity RetrieveEntityById(IOrganizationService service, string EntityLogicalname, Guid guidEntityId)
        {
            Entity entityRecord = service.Retrieve(EntityLogicalname, guidEntityId, new ColumnSet(true));
            return entityRecord;
        }
        public void getEntityRecord(IOrganizationService service, Guid guidEntityId)
        {
            Entity entity = RetrieveEntityById(service, "account", guidEntityId);
            if (entity.Attributes.Contains("name"))
            {
                string strAccountName = entity.Attributes["name"].ToString();
            }

        }
        public void GetEntityRecord(IOrganizationService service)
        {
            ConditionExpression condition1 = new ConditionExpression();
            condition1.AttributeName = "lastname";
            condition1.Operator = ConditionOperator.Equal;
            condition1.Values.Add("Fry");

            ConditionExpression condition2 = new ConditionExpression();
            condition2.AttributeName = "firstname";
            condition2.Operator = ConditionOperator.Equal;
            condition2.Values.Add("Alexis");

            FilterExpression filter1 = new FilterExpression();
            filter1.Conditions.Add(condition1);
            filter1.Conditions.Add(condition2);

            QueryExpression query = new QueryExpression("contact");
            query.ColumnSet.AddColumns("firstname", "lastname");
            query.Criteria.AddFilter(filter1);
            EntityCollection result1 = service.RetrieveMultiple(query);
            if (result1.Entities.Count > 0) //Check for EntityCollection count
            {
                string output = string.Empty;
                foreach (var item in result1.Entities)
                {
                    //String
                    if (item.Attributes.Contains("fullname")) //Check for fullname value exists or not in Entity Collection
                        output += "Full Name : " + item.Attributes["fullname"] + "\n";

                    //Lookup
                    if (item.Attributes.Contains("parentcustomerid")) //Check for parentcustomerid exists or not in Entity Collection
                        output += "Company : " + ((EntityReference)item.Attributes["parentcustomerid"]).Name + "\n";

                    //OptionSet
                    if (item.Attributes.Contains("gendercode")) //Check for gendercode exists or not in Entity Collection
                        output += "Gender : Name - " + item.FormattedValues["gendercode"] + ", Value - " + ((OptionSetValue)item.Attributes["gendercode"]).Value + "\n";

                    //Date
                    if (item.Attributes.Contains("birthdate")) //Check for birthdate exists or not in Entity Collection
                        output += "Birthday : " + ((DateTime)item.Attributes["birthdate"]).ToLocalTime().ToShortDateString().ToString() + "\n";

                    //Currency
                    if (item.Attributes.Contains("creditlimit")) //Check for creditlimit exists or not in Entity Collection
                        output += "Credit Limit : " + ((Money)item.Attributes["creditlimit"]).Value + "\n";

                    //Two Options
                    if (item.Attributes.Contains("donotsendmm")) //Check for donotsendmm exists or not in Entity Collection
                        output += "Send Marketing Materials : Name - " + item.FormattedValues["donotsendmm"] + ", Value - " + ((Boolean)item.Attributes["donotsendmm"]).ToString();
                }
                Console.WriteLine(output);
                Console.ReadKey();
            }
        }
        public void GetLinkedEntityRecord(IOrganizationService service)
        {
            QueryExpression query = new QueryExpression("EntityALogicalName");
            query.ColumnSet = new ColumnSet("column1", "coumn2");
            // Or retrieve All Columns
            //query.ColumnSet = new ColumnSet(true);

            LinkEntity EntityB = new LinkEntity("EntityALogicalName", "EntityBLogicalName", "EntityALinkAttributeName", "EntityBLinkAttributeName", JoinOperator.Inner);
            EntityB.Columns = new ColumnSet("column1", "coumn2");
            EntityB.EntityAlias = "EntityB";
            // Can put condition like this to any Linked entity
            // EntityB.LinkCriteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, 1));
            query.LinkEntities.Add(EntityB);

            // Join Operator can be change if there is chance of Null values in the Lookup. Use Left Outer join
            LinkEntity EntityC = new LinkEntity("EntityALogicalName", "EntityCLogicalName", "EntityALinkAttributeName", "EntityCLinkAttributeName", JoinOperator.Inner);
            EntityC.Columns = new ColumnSet("column1", "coumn2");
            EntityC.Columns = new ColumnSet("column1", "coumn2");
            EntityC.EntityAlias = "EntityC";
            query.LinkEntities.Add(EntityC);

            query.Criteria.Conditions.Add(new ConditionExpression("status", ConditionOperator.Equal, 1));

            var result = service.RetrieveMultiple(query);

            foreach (var entity in result.Entities)
            {
                // Get the Columns from the Entity Obj Like this. Depands on type of the Column. 
                string entityAColumn1 = entity.Contains("column1") ? entity["column1"].ToString() : string.Empty;
                // Use Link Entity Alias with column name
                string entityBColumn1 = entity.Contains("EntityB.column1") ? (entity["EntityB.column1"] as AliasedValue).Value.ToString() : string.Empty;
                string entityCColumn1 = entity.Contains("EntityC.column1") ? (entity["EntityC.column1"] as AliasedValue).Value.ToString() : string.Empty;
            }
        }

        public void FetchXMLExpression(IOrganizationService service)
        {
            string fetchquery = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='account'>
                                    <attribute name='name' />
                                    <attribute name='primarycontactid' />
                                    <attribute name='telephone1' />
                                    <attribute name='accountid' />
                                    <order attribute='name' descending='false' />
                                  </entity>
                                </fetch>";

            EntityCollection accounts = service.RetrieveMultiple(new FetchExpression(fetchquery));
            foreach (var c in accounts.Entities)
            {
                Console.WriteLine("Name: {0}", c.Attributes["name"]);
            }

        }

        public void FetchXMLExpressionAliasedValue(IOrganizationService service)
        {
            string estimatedvalue_avg = @" 
            <fetch distinct='false' mapping='logical' aggregate='true'> 
	            <entity name='opportunity'> 
	               <attribute name='estimatedvalue' alias='estimatedvalue_avg' aggregate='avg' /> 
	            </entity> 
            </fetch>";

            EntityCollection estimatedvalue_avg_result = service.RetrieveMultiple(new FetchExpression(estimatedvalue_avg));

            foreach (var c in estimatedvalue_avg_result.Entities)
            {
                decimal aggregate1 = ((Money)((AliasedValue)c["estimatedvalue_avg"]).Value).Value;
                System.Console.WriteLine("Average estimated value: " + aggregate1);

            }
        }
        public void QueryByAttribute(IOrganizationService service, string accountid)
        {
            QueryByAttribute querybyexpression = new QueryByAttribute("account");
            querybyexpression.ColumnSet = new ColumnSet("name", "address1_city", "emailaddress1");

            // Attribute to query
            querybyexpression.Attributes.AddRange("name");

            // Value of queried attribute to return
            querybyexpression.Values.AddRange("Peter");
        }
         public void QueryExpressionpaginatequery(IOrganizationService service,string accountid)
        {
            // Query using the paging cookie.
            // Define the paging attributes.
            // The number of records per page to retrieve.
            int queryCount = 3;

            // Initialize the page number.
            int pageNumber = 1;

            // Initialize the number of records.
            int recordCount = 0;

            // Define the condition expression for retrieving records.
            ConditionExpression pagecondition = new ConditionExpression();
            pagecondition.AttributeName = "parentaccountid";
            pagecondition.Operator = ConditionOperator.Equal;
            pagecondition.Values.Add(accountid);

            // Define the order expression to retrieve the records.
            OrderExpression order = new OrderExpression();
            order.AttributeName = "name";
            order.OrderType = OrderType.Ascending;

            // Create the query expression and add condition.
            QueryExpression pagequery = new QueryExpression();
            pagequery.EntityName = "account";
            pagequery.Criteria.AddCondition(pagecondition);
            pagequery.Orders.Add(order);
            pagequery.ColumnSet.AddColumns("name", "emailaddress1");

            // Assign the pageinfo properties to the query expression.
            pagequery.PageInfo = new PagingInfo();
            pagequery.PageInfo.Count = queryCount;
            pagequery.PageInfo.PageNumber = pageNumber;

            // The current paging cookie. When retrieving the first page, 
            // pagingCookie should be null.
            pagequery.PageInfo.PagingCookie = null;
            Console.WriteLine("Retrieving sample account records in pages...\n");
            Console.WriteLine("#\tAccount Name\t\tEmail Address");

            while (true)
            {
                // Retrieve the page.
                EntityCollection results = service.RetrieveMultiple(pagequery);
                if (results.Entities != null)
                {
                    // Retrieve all records from the result set.
                    foreach (var acct in results.Entities)
                    {
                        Console.WriteLine("{0}.\t{1}\t{2}", ++recordCount, acct["Name"],
                                           acct["EMailAddress1"]);
                    }
                }

                // Check for more records, if it returns true.
                if (results.MoreRecords)
                {
                    Console.WriteLine("\n****************\nPage number {0}\n****************", pagequery.PageInfo.PageNumber);
                    Console.WriteLine("#\tAccount Name\t\tEmail Address");

                    // Increment the page number to retrieve the next page.
                    pagequery.PageInfo.PageNumber++;

                    // Set the paging cookie to the paging cookie returned from current results.
                    pagequery.PageInfo.PagingCookie = results.PagingCookie;
                }
                else
                {
                    // If no more records are in the result nodes, exit the loop.
                    break;
                }
            }
        }
        public string CreateXml(string xml, string cookie, int page, int count)
        {
            StringReader stringReader = new StringReader(xml);
            XmlTextReader reader = new XmlTextReader(stringReader);

            // Load document
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            return CreateXml(doc, cookie, page, count);
        }

        public string CreateXml(XmlDocument doc, string cookie, int page, int count)
        {
            XmlAttributeCollection attrs = doc.DocumentElement.Attributes;

            if (cookie != null)
            {
                XmlAttribute pagingAttr = doc.CreateAttribute("paging-cookie");
                pagingAttr.Value = cookie;
                attrs.Append(pagingAttr);
            }

            XmlAttribute pageAttr = doc.CreateAttribute("page");
            pageAttr.Value = System.Convert.ToString(page);
            attrs.Append(pageAttr);

            XmlAttribute countAttr = doc.CreateAttribute("count");
            countAttr.Value = System.Convert.ToString(count);
            attrs.Append(countAttr);

            StringBuilder sb = new StringBuilder(1024);
            StringWriter stringWriter = new StringWriter(sb);

            XmlTextWriter writer = new XmlTextWriter(stringWriter);
            doc.WriteTo(writer);
            writer.Close();

            return sb.ToString();
        }
        public void paginatefetchxmlquery(IOrganizationService service, string accountid)
        {
            // Define the fetch attributes.
            // Set the number of records per page to retrieve.
            int fetchCount = 3;
            // Initialize the page number.
            int pageNumber = 1;
            // Initialize the number of records.
            int recordCount = 0;
            // Specify the current paging cookie. For retrieving the first page, 
            // pagingCookie should be null.
            string pagingCookie = null;

            // Create the FetchXml string for retrieving all child accounts to a parent account.
            // This fetch query is using 1 placeholder to specify the parent account id 
            // for filtering out required accounts. Filter query is optional.
            // Fetch query also includes optional order criteria that, in this case, is used 
            // to order the results in ascending order on the name data column.
            string fetchXml = string.Format(@"<fetch version='1.0' 
                                                    mapping='logical' 
                                                    output-format='xml-platform'>
                                                    <entity name='account'>
                                                        <attribute name='name' />
                                                        <attribute name='emailaddress1' />
                                                        <order attribute='name' descending='false'/>
                                                        <filter type='and'>
                                                            <condition attribute='parentaccountid' 
                                                                operator='eq' value='{0}' uiname='' uitype='' />
                                                        </filter>
                                                    </entity>
                                                </fetch>",
                                            accountid);

            Console.WriteLine("Retrieving data in pages\n");
            Console.WriteLine("#\tAccount Name\t\t\tEmail Address");

            while (true)
            {
                // Build fetchXml string with the placeholders.
                string xml = CreateXml(fetchXml, pagingCookie, pageNumber, fetchCount);

                // Excute the fetch query and get the xml result.
                RetrieveMultipleRequest fetchRequest1 = new RetrieveMultipleRequest
                {
                    Query = new FetchExpression(xml)
                };

                EntityCollection returnCollection = ((RetrieveMultipleResponse)service.Execute(fetchRequest1)).EntityCollection;

                foreach (var c in returnCollection.Entities)
                {
                    System.Console.WriteLine("{0}.\t{1}\t\t{2}", ++recordCount, c.Attributes["name"], c.Attributes["emailaddress1"]);
                    
                
                
                }

                // Check for morerecords, if it returns 1.
                if (returnCollection.MoreRecords)
                {
                    Console.WriteLine("\n****************\nPage number {0}\n****************", pageNumber);
                    Console.WriteLine("#\tAccount Name\t\t\tEmail Address");

                    // Increment the page number to retrieve the next page.
                    pageNumber++;

                    // Set the paging cookie to the paging cookie returned from current results.                            
                    pagingCookie = returnCollection.PagingCookie;
                }
                else
                {
                    // If no more records in the result nodes, exit the loop.
                    break;
                }
            }
        }
            public void DeleteRecord(IOrganizationService service, string accountId)
        {
            QueryExpression oppQuery = new QueryExpression("opportunity");
            oppQuery.ColumnSet = new ColumnSet(new string[] { "opportunityid" });
            oppQuery.Criteria.AddCondition(new ConditionExpression("parentcontactid", ConditionOperator.Equal, accountId));

            EntityCollection opportunities = service.RetrieveMultiple(oppQuery);

            foreach (var opportunity in opportunities.Entities)
            {
                service.Delete("opportunity", opportunity.Id);
            }
        }
            public void FetchXMLExpressionLinked(IOrganizationService service,string accountId)
        {
            var contactFirstName = default(object);
            var contactLastName = default(object);
            var contactFullName = default(object);

            string fetchXML = string.Format(@"<fetch version='1.0' output-format='xml-platform' no-lock='true' mapping='logical'>
                                        <entity name='account'>
                                            <attribute name='name' />                                                                                
                                            <filter type='and'>
                                                <condition attribute='statuscode' operator='eq' value='1' />                                            
                                                <condition attribute='accountid' operator='eq' value='{0}' />
                                            </filter>                                        
                                            <link-entity name='contact' from='contactid' to='primarycontactid' alias='ab'>
                                                 <attribute name='fullname' alias='as_fullname' />
                                                 <attribute name='firstname' alias='as_firstname' />                                             
                                                 <attribute name='lastname' alias='as_lastname' />
                                            </link-entity>
                                        </entity>
                                    </fetch>", accountId.ToString());

            var fetchExp = new FetchExpression(fetchXML);

            EntityCollection accountEntity = service.RetrieveMultiple(fetchExp);

            if (accountEntity.Entities.Count > 0)
            {
                //Primary Contact Fullname
                AliasedValue avContactFullname = accountEntity.Entities[0].GetAttributeValue<AliasedValue>("as_fullname");
                if (avContactFullname != null)
                    contactFullName = avContactFullname.Value;
                //Primary Contact Firstname
                AliasedValue avContactFirstname = accountEntity.Entities[0].GetAttributeValue<AliasedValue>("as_firstname");
                if (avContactFirstname != null)
                    contactFirstName = avContactFirstname.Value;
                //Primary Contact Lastname
                AliasedValue avContactLastname = accountEntity.Entities[0].GetAttributeValue<AliasedValue>("as_lastname");
                if (avContactLastname != null)
                    contactLastName = avContactLastname.Value;
                //to get optionset value
                int statusValue = ((OptionSetValue)accountEntity.Entities[0].GetAttributeValue<AliasedValue>("statusoption").Value).Value;

                var AccountLookupName = ((EntityReference)(accountEntity.Entities[0].GetAttributeValue<AliasedValue>("accountid").Value)).Name;
                var AccountLookupID = ((EntityReference)(accountEntity.Entities[0].GetAttributeValue<AliasedValue>("accountid").Value)).Id;



            }
        }
       
        private static EntityCollection GetEntityCollection(IOrganizationService service, string entityName, string attributeName, string attributeValue, ColumnSet cols)
        {
            QueryExpression query = new QueryExpression
            {
                EntityName = entityName,
                ColumnSet = cols,
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                    new ConditionExpression
                    {
                    AttributeName = attributeName,
                    Operator = ConditionOperator.Equal,
                    Values = { attributeValue }
                    }
                    }
                }
            };
            return service.RetrieveMultiple(query);
        }

    }
}

using System;
using System.Activities;
using System.Collections.ObjectModel;

using Microsoft.Crm.Sdk.Messages;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;

namespace Custom_Workflow
{
    public class Program : CodeActivity
    {
        //Data Type : String 
        [Input("patientname")]
        [RequiredArgument]
        public InArgument<string> PatientName { get; set; }

        //Data Type : Date and Time
        [Input("dateofbirth")]
        [RequiredArgument]
        public InArgument DateOfBirth { get; set; }

        //Data Type : Lookup
        [Input("hospital")]
        [ReferenceTarget("new_hospital")]
        [RequiredArgument]
        public InArgument Hospital { get; set; }

        //Data Type : Option Set
        [Input("patientstatus")]
        [RequiredArgument]
        [AttributeTarget("new_patient", "statuscode")]
        public InArgument PatientStatus { get; set; }

        //Data Type : Two Option Set
        [Input("hospitalizationrequired")]
        [RequiredArgument]
        public InArgument<bool> HospitalizationRequired { get; set; }

        //Data Type : Whole Number
        [Input("patientage")]
        [RequiredArgument]
        public InArgument<int> PatientAge { get; set; }

        //Data Type : Decimal Number
        [Input("consultationfee")]
        [RequiredArgument]
        public InArgument<decimal> ConsultationFee { get; set; }

        //Data Type : Floating Point Number
        [Input("estimatedamount")]
        [RequiredArgument]
        public InArgument<decimal> EstimatedAmount { get; set; }

        //Data Type : Currency
        [Input("treatmentcost")]
        [RequiredArgument]
        public InArgument TreatmentCost { get; set; }

        //Data Type : String
        [Output("showpatientdetails")]
        public OutArgument<string> ShowPatientDetails { get; set; }


        //Data Type : String (Can be used as Input/Output)
        [Input("patientinput")]
        [Output("patientoutput")]
        [RequiredArgument]
        public InOutArgument<string> PatientInOut { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            ITracingService tracingService = (ITracingService)context.GetExtension<ITracingService>();
            IWorkflowContext workflowContext = (IWorkflowContext)context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.UserId);
            tracingService.Trace("Patient Details using input and output parameters Workflow Started.");
            var patientName = PatientName.Get(context);
            var dateOfBirth = DateOfBirth.Get(context);
            var hospital = ((EntityReference)(Hospital.Get(context))).Id;
            var hospitalid = ((EntityReference)(Hospital.Get(context))).Name;
            var patientStatus = PatientStatus.Get(context);
            var hospitalizationRequired = HospitalizationRequired.Get(context);
            var patientAge = PatientAge.Get(context);
            var consultationFee = ConsultationFee.Get(context);
            var estimatedAmount = EstimatedAmount.Get(context);
            var treatmentCost = ((OptionSetValue)(TreatmentCost.Get(context))).Value;
            var patientId = PatientInOut.Get(context);
            tracingService.Trace($"Patient Name : {patientName}, Date Of Birth : {dateOfBirth}, Hospital : {hospital}, Patient Status : {patientStatus}, Hospitalization Required: {hospitalizationRequired}, Patient Age: {patientAge}, Consultation Fee : {consultationFee}, Estimated Amount : {estimatedAmount}, Treatment Cost : {treatmentCost}, Patient ID : {patientId}");
            var patientDetails = $"Patient Name : {patientName}, Date Of Birth : {dateOfBirth}, Hospital : {hospital}, Patient Status : {patientStatus}, Hospitalization Required: {hospitalizationRequired}, Patient Age: {patientAge}, Consultation Fee : {consultationFee}, Estimated Amount : {estimatedAmount}, Treatment Cost : {treatmentCost}, Patient ID : {patientId}";
            PatientInOut.Set(context, PatientInOut.ToString());
            ShowPatientDetails.Set(context, patientDetails);
            tracingService.Trace("Patient Details using input and output parameters Workflow Ended.");

        }
    
      
    }
   
}

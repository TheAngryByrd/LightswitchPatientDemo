using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Security.Server;
namespace LightSwitchApplication
{
    public partial class ApplicationDataService
    {


        partial void CurrentPatients_PreprocessQuery(bool? SelectOnlyCurrent, ref IQueryable<Patient> query)
        {

            if (SelectOnlyCurrent.HasValue && SelectOnlyCurrent.Value) {
                var sixWeeksAgo =DateTime.Now.Date.AddDays(-42);
                //Get All patients that have been treated within the last 6 weeks 
                query = from patient in query
                        from treament in patient.Treatments
                        where treament.DateTreated >= sixWeeksAgo
                        select patient;
  
            }
        }

        partial void LabsBetweenTreatments_PreprocessQuery(int? TreatmentId, ref IQueryable<SpecificLabs> query)
        {
            var treatment = Treatments.Where(a => a.Id == TreatmentId).Single();

            //look for the next treatment
            var nextTreatment = Treatments.Where(b => b.DateTreated > treatment.DateTreated).OrderBy(a => a.DateTreated).FirstOrDefault();

            //if there is a treatment, get all labs between the two
            if (nextTreatment != null)
            {
                query = from lab in query
                        where lab.DateLabIssued >= treatment.DateTreated && lab.DateLabIssued <= nextTreatment.DateTreated
                        select lab;
            }
            //else just get the rest of the labs starting from that treatment
            else
            {
                query = from lab in query
                        where lab.DateLabIssued >= treatment.DateTreated
                        select lab;
            }

        }
    }
}

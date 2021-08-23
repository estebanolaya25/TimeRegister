using System;
using System.IO;
using System.Threading.Tasks;
using TimeRegister.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using TimeRegister.Common.Responses;
using TimeRegister.Functions.Entities;
using System.Collections.Generic;

namespace TimeRegister.Functions.Functions
{
    public static class TimeRegiterApi
    {
        [FunctionName(nameof(CreateTimeRegister))]
        public static async Task<IActionResult> CreateTimeRegister(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "timeregister")] HttpRequest req,
            [Table("timeregister", Connection = "AzureWebJobsStorage")] CloudTable timeregisterTable,
            ILogger log)
        {
            log.LogInformation("Recieved a new Time Register");         

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Timeregister Timeregister=JsonConvert.DeserializeObject<Timeregister>(requestBody);
            int Type;
            if (Timeregister==null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "The request must have all Items"
                });
            }

            if ((Timeregister.EntryDate == null) && (Timeregister.DepartureDate == null))
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "The request must have EntryDate or DepartureDate"
                });
            }
            if (Timeregister.EntryDate == null)
            {
                Type = 1;
            }
            else {
                Type = 0;
            }

            TimeregisterEntity timeregisterEntity= new TimeregisterEntity
             {
                 EmployeeId= Timeregister.EmployeeId,
                 TypeEntry = Type,
                  Consolidated = false,
                 ETag ="*",
                 EntryDate=Timeregister.EntryDate,
                 DepartureDate=Timeregister.DepartureDate,
                 PartitionKey = "TimeRegister",
                 RowKey = Guid.NewGuid().ToString()

                
                                 
            };

            TableOperation addOperation = TableOperation.Insert(timeregisterEntity);
            await timeregisterTable.ExecuteAsync(addOperation);
            string message = "New todo stored in table";
            log.LogInformation(message);
            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = timeregisterEntity
            });
        }
        [FunctionName(nameof(GetAlltimeregister))]
        public static async Task<IActionResult> GetAlltimeregister(
   [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "timeregister/{IdEmpleado}")] HttpRequest req,
   [Table("timeregister", Connection = "AzureWebJobsStorage")] CloudTable timeregisterTable,int IdEmpleado,
   ILogger log)
        {
            log.LogInformation("Get all todos received.");

            TableQuery<TimeregisterEntity> query = new TableQuery<TimeregisterEntity>();
            TableQuerySegment<TimeregisterEntity> timeregister = await timeregisterTable.ExecuteQuerySegmentedAsync(query, null);

            List<Timeregister> listTimeregister = new List<Timeregister>();

            foreach (TimeregisterEntity lst in timeregister)
            {
                if (lst.EmployeeId == IdEmpleado)
                {
                    Timeregister onjtimer = new Timeregister();
                    onjtimer.EmployeeId = lst.EmployeeId;
                    onjtimer.EntryDate= lst.EntryDate;
                    onjtimer.DepartureDate= lst.DepartureDate;
                    listTimeregister.Add(onjtimer);

                }
            
            }

            string message = "Retrieved all timeregister";
            log.LogInformation(message);
            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = listTimeregister
            });
        }

    }
}

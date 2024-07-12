using System;
using System.Collections.Generic;
using System.Linq;

namespace Vit.Linq.ExpressionTree.MsTest
{
    public class Person
    {
        public int id;
        public int? departmentId;
        public string name { get; set; }
        public DateTime addTime;
        public string ext;
        public bool isEven;

        public Job job;

        public Job[] jobArray;

        public List<Job> jobList;

        public Person PopulateJob()
        {
            job = new Job { departmentId = departmentId, personId = id, name = name + "_job1" };
            var job2 = new Job { departmentId = departmentId, personId = id, name = name + "_job2" };


            if (id % 2 == 0)
                jobArray = new[] { job, job2 };
            else
                jobArray = new[] { job };

            jobList = jobArray.ToList();

            return this;
        }

        public int GetJobCount()
        {
            return jobList.Count;
        }
        public bool JobExistAtIndex(int index)
        {
            if (index < jobList.Count) return true;
            return false;
        }
        public Job GetJobAtIndex(int index)
        {
            if (index < jobList.Count) return jobList[index];
            return null;
        }
    }

    public class Job
    {
        public int? departmentId;

        public int? personId;
        public string name;
        public string GetJobName()
        {
            return name;
        }
    }

}

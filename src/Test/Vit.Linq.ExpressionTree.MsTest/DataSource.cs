using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Vit.Linq.MsTest
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



    public class DataSource
    {
        public static List<Person> BuildDataSource(int count = 1000)
        {
            var Now = DateTime.Now;
            var list = new List<Person>(count);
            for (int i = 0; i < count; i++)
            {
                list.Add(new Person
                {
                    id = i,
                    departmentId = i / 10,
                    name = "name" + i,
                    addTime = Now.AddSeconds(i),
                    ext = "ext" + i,
                    isEven = i % 2 == 0
                }.PopulateJob());

            }
            return list;
        }

        public static IQueryable GetIQueryable() => GetQueryable();
        public static IQueryable<Person> GetQueryable() => BuildDataSource().AsQueryable();
    }
}

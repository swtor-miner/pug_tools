using System;
using System.Collections.Generic;
using TreeViewFast.Entities;


namespace TreeViewFast.DomainServices
{
    public static class EmployeeGenerator
    {
        private static readonly string[] FirstNames = { "Anna", "Brian", "Cecilia", "David", "Eva", "Fredric", "George", "Harald", "Inez", "Jacob", "Katarina", "Leonard", "Marianne" };
        private static readonly string[] LastNames = { "Andersson", "Brown", "Collins", "Davis", "Garcia", "Johnson", "King", "Martinez", "Robinson", "Smith", "Taylor", "White" };
        private static readonly Random MyRandom = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// Generate a number of employees for demo usage.
        /// </summary>
        public static List<Employee> GetEmployees(int count)
        {
            var employees = new List<Employee>();
            for (int i = 0; i < count; i++)
            {
                var employee = new Employee
                {
                    EmployeeId = i,
                    ManagerId = i > 20 ? employees[MyRandom.Next(0, i - 1)].EmployeeId : (int?)null,
                    FirstName = FirstNames[MyRandom.Next(0, FirstNames.Length - 1)],
                    LastName = LastNames[MyRandom.Next(0, LastNames.Length - 1)]
                };
                employees.Add(employee);
            }
            return employees;
        }
    }
}

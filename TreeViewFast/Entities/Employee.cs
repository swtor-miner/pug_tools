namespace TreeViewFast.Entities
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public int? ManagerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Name
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }
    }
}
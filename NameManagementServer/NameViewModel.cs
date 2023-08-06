using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NameManagementServer
{
    internal class NameViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Record> records;

        public ObservableCollection<Record> Records
        {
            get { return records; }
            set { records = value; OnPropertyChanged(); }
        }

        // Constructor
        public NameViewModel()
        {
            // Initialize the collection or load data from CSV file
            // For simplicity, I'm just creating some sample data here
            records = new ObservableCollection<Record>
            {
                new Record { ID = "1", FirstName = "John", LastName = "Doe" },
                new Record { ID = "2", FirstName = "Jane", LastName = "Smith" }
            };
        }

        // Add a new name record
        public void AddRecord(Record record)
        {
            records.Add(record);
            // SaveRecordsToCSV(); // Implement this method to save data to CSV file
        }

       // Delete a record
        public void DeleteRecord(Record record)
        {
            records.Remove(record);
            // SaveRecordsToCSV(); // Implement this method to save data to CSV file
        }

        // Other CRUD operations (Read, Update) can be implemented similarly

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
}

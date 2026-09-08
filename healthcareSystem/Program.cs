using System;
using System.Collections.Generic;
using System.Linq;

// 1. Generic Repository

public class Repository<T>
{
    private List<T> items = new List<T>();

    // Add an item to the repository
    public void Add(T item)
    {
        items.Add(item);
    }

    // Get all items
    public List<T> GetAll()
    {
        return new List<T>(items);
    }

    // Get the first item that satisfies the condition
    public T? GetById(Func<T, bool> predicate)
    {
        return items.FirstOrDefault(predicate);
    }

    // Remove the first item that satisfies the condition
    public bool Remove(Func<T, bool> predicate)
    {
        T? item = items.FirstOrDefault(predicate);

        if (item != null)
        {
            return items.Remove(item);
        }

        return false;
    }
}



// 2. Patient Class
// =====================================================

public class Patient
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }


    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
    }

    public override string ToString()
    {
        return $"ID: {Id}, Name: {Name}, Age: {Age}, Gender: {Gender}";
    }
}



// 3. Prescription Class
// =====================================================

public class Prescription
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string MedicationName { get; set; }
    public DateTime DateIssued { get; set; }


    public Prescription(
        int id,
        int patientId,
        string medicationName,
        DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;
        MedicationName = medicationName;
        DateIssued = dateIssued;
    }

    public override string ToString()
    {
        return $"Prescription ID: {Id}, " +
               $"Medication: {MedicationName}, " +
               $"Date Issued: {DateIssued:yyyy-MM-dd}";
    }
}


// 4. Healthcare System Application
// =====================================================

public class HealthSystemApp
{
    // Repository for patients
    private Repository<Patient> _patientRepo;

    // Repository for prescriptions
    private Repository<Prescription> _prescriptionRepo;


    // Value = List of prescriptions belonging to that patient
    private Dictionary<int, List<Prescription>> _prescriptionMap;


    // Constructor
    public HealthSystemApp()
    {
        _patientRepo = new Repository<Patient>();
        _prescriptionRepo = new Repository<Prescription>();

        _prescriptionMap =
            new Dictionary<int, List<Prescription>>();
    }



    // SeedData()
    // Adds patients and prescriptions
    // =================================================

    public void SeedData()
    {

        // Add Patients
        // -------------------------

        _patientRepo.Add(
            new Patient(
                1,
                "John Mensah",
                35,
                "Male"
            )
        );

        _patientRepo.Add(
            new Patient(
                2,
                "Ama Boateng",
                28,
                "Female"
            )
        );

        _patientRepo.Add(
            new Patient(
                3,
                "Kofi Asare",
                45,
                "Male"
            )
        );



        // Add Prescriptions
        // -------------------------

        _prescriptionRepo.Add(
            new Prescription(
                101,
                1,
                "Paracetamol",
                new DateTime(2026, 9, 1)
            )
        );

        _prescriptionRepo.Add(
            new Prescription(
                102,
                1,
                "Amoxicillin",
                new DateTime(2026, 9, 2)
            )
        );

        _prescriptionRepo.Add(
            new Prescription(
                103,
                2,
                "Ibuprofen",
                new DateTime(2026, 9, 3)
            )
        );

        _prescriptionRepo.Add(
            new Prescription(
                104,
                2,
                "Vitamin C",
                new DateTime(2026, 9, 4)
            )
        );

        _prescriptionRepo.Add(
            new Prescription(
                105,
                3,
                "Cetirizine",
                new DateTime(2026, 9, 5)
            )
        );
    }



    // BuildPrescriptionMap()
    // Groups prescriptions by PatientId
    // =================================================

    public void BuildPrescriptionMap()
    {
        _prescriptionMap.Clear();

        // Get all prescriptions from repository
        List<Prescription> prescriptions =
            _prescriptionRepo.GetAll();

        // Loop through all prescriptions
        foreach (Prescription prescription in prescriptions)
        {
            // If the patient ID doesn't exist in the dictionary,
            // create a new list for that patient.
            if (!_prescriptionMap.ContainsKey(prescription.PatientId))
            {
                _prescriptionMap[prescription.PatientId] =
                    new List<Prescription>();
            }

            // Add prescription to the patient's list
            _prescriptionMap[prescription.PatientId]
                .Add(prescription);
        }
    }


    // Displays all patients
    // =================================================

    public void PrintAllPatients()
    {
        Console.WriteLine("\n===== ALL PATIENTS =====");

        List<Patient> patients =
            _patientRepo.GetAll();

        foreach (Patient patient in patients)
        {
            Console.WriteLine(patient);
        }
    }



    // Retrieves prescriptions from dictionary
    // =================================================

    public List<Prescription> GetPrescriptionsByPatientId(
        int patientId)
    {
        if (_prescriptionMap.TryGetValue(
            patientId,
            out List<Prescription>? prescriptions))
        {
            return prescriptions;
        }

        // Return an empty list if no prescriptions exist
        return new List<Prescription>();
    }

    // Displays prescriptions for a patient
    // =================================================

    public void PrintPrescriptionsForPatient(int patientId)
    {
        Console.WriteLine(
            $"\n===== PRESCRIPTIONS FOR PATIENT {patientId} ====="
        );

        // Find the patient
        Patient? patient =
            _patientRepo.GetById(p => p.Id == patientId);

        if (patient == null)
        {
            Console.WriteLine("Patient not found.");
            return;
        }

        Console.WriteLine($"Patient: {patient.Name}");
        Console.WriteLine();

        // Get prescriptions from dictionary
        List<Prescription> prescriptions =
            GetPrescriptionsByPatientId(patientId);

        if (prescriptions.Count == 0)
        {
            Console.WriteLine("No prescriptions found.");
            return;
        }

        foreach (Prescription prescription in prescriptions)
        {
            Console.WriteLine(prescription);
        }
    }
}



// 5. Main Application
// =====================================================

public class Program
{
    public static void Main()
    {
        // i. Instantiate HealthSystemApp
        HealthSystemApp app = new HealthSystemApp();

        // ii. Call SeedData()
        app.SeedData();

        // iii. Build prescription dictionary
        app.BuildPrescriptionMap();

        // iv. Print all patients
        app.PrintAllPatients();

        // v. Select a PatientId and display prescriptions
        int selectedPatientId = 1;

        app.PrintPrescriptionsForPatient(
            selectedPatientId
        );

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
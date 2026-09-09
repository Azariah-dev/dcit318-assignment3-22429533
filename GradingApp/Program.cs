using System;
using System.Collections.Generic;
using System.IO;

namespace SchoolGradingSystem
{
    // Student class
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int Score { get; set; }

        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName;
            Score = score;
        }

        // Method to assign grade
        public string GetGrade()
        {
            if (Score >= 80 && Score <= 100)
                return "A";
            else if (Score >= 70 && Score <= 79)
                return "B";
            else if (Score >= 60 && Score <= 69)
                return "C";
            else if (Score >= 50 && Score <= 59)
                return "D";
            else
                return "F";
        }
    }


    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message)
            : base(message)
        {
        }
    }

    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message)
            : base(message)
        {
        }
    }


    // Student result processor class
    public class StudentResultProcessor
    {

        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            List<Student> students = new List<Student>();

            using (StreamReader reader = new StreamReader(inputFilePath))
            {
                string? line;

                while ((line = reader.ReadLine()) != null)
                {

                    if (string.IsNullOrWhiteSpace(line))
                        continue;


                    string[] fields = line.Split(',');


                    if (fields.Length != 3)
                    {
                        throw new MissingFieldException(
                            $"Incomplete student record: {line}"
                        );
                    }


                    string idText = fields[0].Trim();
                    string fullName = fields[1].Trim();
                    string scoreText = fields[2].Trim();


                    if (string.IsNullOrWhiteSpace(idText) ||
                        string.IsNullOrWhiteSpace(fullName) ||
                        string.IsNullOrWhiteSpace(scoreText))
                    {
                        throw new MissingFieldException(
                            $"A required field is missing: {line}"
                        );
                    }


                    if (!int.TryParse(idText, out int id))
                    {
                        throw new FormatException(
                            $"Invalid student ID: {idText}"
                        );
                    }


                    if (!int.TryParse(scoreText, out int score))
                    {
                        throw new InvalidScoreFormatException(
                            $"Invalid score format for student {fullName}: {scoreText}"
                        );
                    }


                    if (score < 0 || score > 100)
                    {
                        throw new InvalidScoreFormatException(
                            $"Score must be between 0 and 100 for {fullName}: {score}"
                        );
                    }


                    Student student = new Student(id, fullName, score);

                    students.Add(student);
                }
            }

            return students;
        }


        public void WriteReportToFile(
            List<Student> students,
            string outputFilePath)
        {
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                foreach (Student student in students)
                {
                    writer.WriteLine(
                        $"{student.FullName} (ID: {student.Id}): " +
                        $"Score = {student.Score}, Grade = {student.GetGrade()}"
                    );
                }
            }
        }
    }


    // Main application
    class Program
    {
        static void Main(string[] args)
        {
            string inputFilePath = "students.txt";
            string outputFilePath = "student_report.txt";

            try
            {
                StudentResultProcessor processor =
                    new StudentResultProcessor();


                List<Student> students =
                    processor.ReadStudentsFromFile(inputFilePath);


                processor.WriteReportToFile(
                    students,
                    outputFilePath
                );

                Console.WriteLine(
                    $"Student report successfully created: {outputFilePath}"
                );
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(
                    "Error: The input file could not be found."
                );
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine(
                    $"Invalid Score Error: {ex.Message}"
                );
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine(
                    $"Missing Field Error: {ex.Message}"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"An unexpected error occurred: {ex.Message}"
                );
            }
        }
    }
}
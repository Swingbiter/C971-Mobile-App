using System;
using System.Collections.Generic;
using System.ComponentModel;
using SQLite;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.LocalNotifications;
using C971_Mobile_App.AddEditForms;

namespace C971_Mobile_App
{
    // Database table definitions
    [Table("Term")]
    public class Term
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
    [Table("Course")]
    public class Course
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int Term { get; set; } // refers to course's term
        public string CourseName { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Instructor { get; set; }
        public string InstructorPhone { get; set; }
        public string InstructorEmail { get; set; }
        public string Notes { get; set; }
        public bool Notification { get; set; }
    }
    [Table("Assessment")]
    public class Assessment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int Course { get; set; } // refers to assessment's course
        public string Title { get; set; }
        public string Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Notification { get; set; }
    }
    public partial class MainPage : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        public ObservableCollection<Term> term_List;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                //  create tables
                conn.CreateTable<Term>();
                conn.CreateTable<Course>();
                conn.CreateTable<Assessment>();

                //  turn tables into lists
                var terms = conn.Table<Term>().ToList();
                var courses = conn.Table<Course>().ToList();
                var assessments = conn.Table<Assessment>().ToList();

                //  if terms empty, make fake data
                if (!terms.Any())
                {
                    //  fake term
                    var fakeTerm = new Term();
                    fakeTerm.Title = "Term 1";
                    fakeTerm.StartDate = new DateTime(2022, 02, 09);
                    fakeTerm.EndDate = new DateTime(2023, 03, 02);
                    conn.Insert(fakeTerm);
                    terms.Add(fakeTerm);
                    //  fake course
                    var fakeCourse = new Course();
                    fakeCourse.Term = fakeTerm.Id; // link to term
                    fakeCourse.CourseName = "Underwater Basket Weaving";
                    fakeCourse.StartDate = new DateTime(2022, 02, 12);
                    fakeCourse.EndDate = new DateTime(2022, 06, 13);
                    fakeCourse.Status = "Underwater and In-Progress";
                    fakeCourse.Instructor = "Brady Starling";
                    fakeCourse.InstructorPhone = "555-555-5555"; // not gonna put down my personal phone number.
                    fakeCourse.InstructorEmail = "bstarl8@wgu.edu";
                    fakeCourse.Notification = true;
                    fakeCourse.Notes = "It's hard to breathe but easy to weave!";
                    conn.Insert(fakeCourse);
                    //  fake assessments
                    //      fake objective assessment
                    var fakeObjAssess = new Assessment();
                    fakeObjAssess.Course = fakeCourse.Id; // link to course
                    fakeObjAssess.Title = "Practical Weaving Knowledge";
                    fakeObjAssess.StartDate = new DateTime(2022, 03, 15);
                    fakeObjAssess.EndDate = new DateTime(2022, 09, 25);
                    fakeObjAssess.Type = "Objective";
                    fakeObjAssess.Notification = true;
                    conn.Insert(fakeObjAssess);
                    //      fake performance assessment
                    var fakePerAssess = new Assessment();
                    fakePerAssess.Course = fakeCourse.Id; // link to course
                    fakePerAssess.Title = "Basket Magnum Opus";
                    fakePerAssess.StartDate = new DateTime(2022, 02, 09);
                    fakePerAssess.EndDate = new DateTime(2022, 08, 30);
                    fakePerAssess.Type = "Performance";
                    fakePerAssess.Notification = true;
                    conn.Insert(fakePerAssess);
                }

                listView_Terms.ItemsSource = terms;

                // Notifications for terms not requested and therefore left out
                // Notifications notify if event happens within 5 days
                // Notifications for Courses
                int course_num = 0;
                foreach (Course course in courses)
                {
                    if (course.Notification)
                    {
                        if ((DateTime.Today <= course.StartDate) && (course.StartDate <= DateTime.Today.AddDays(5)))
                        {
                            CrossLocalNotifications.Current.Show("Course Starts Soon!", $"{course.CourseName} starts soon!", course_num, DateTime.Now.AddSeconds(1));
                        }
                        if ((DateTime.Today <= course.EndDate) && (course.EndDate <= DateTime.Today.AddDays(5)))
                        {
                            CrossLocalNotifications.Current.Show("Course Ends Soon!", $"{course.CourseName} ends soon!", course_num, DateTime.Now.AddSeconds(1));
                        }
                        course_num++;
                    }
                }

                // Notifications for Assessments
                int assessment_num = 0;
                foreach (Assessment assessment in assessments)
                {
                    if (assessment.Notification)
                    {
                        if ((DateTime.Today <= assessment.StartDate) && (assessment.StartDate <= DateTime.Today.AddDays(5)))
                        {
                            CrossLocalNotifications.Current.Show("Assessment starts today!", $"{assessment.Title} starts today!", assessment_num, DateTime.Now.AddSeconds(1));
                        }
                        if ((DateTime.Today <= assessment.EndDate) && (assessment.EndDate <= DateTime.Today.AddDays(5)))
                        {
                            CrossLocalNotifications.Current.Show("Assessment due today!", $"{assessment.Title} due today!", assessment_num, DateTime.Now.AddSeconds(1));
                        }
                        assessment_num++;
                    }
                }
            }
        }

        private void btn_addTerm_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddEditTerm());
        }

        private void listView_Terms_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Term term = (Term)e.Item;
            Navigation.PushAsync(new CoursesPage(term));
        }

        private async void btn_del_all_data_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayAlert("Delete All Data?", "Are you sure?", "Yes", "Cancel");
            if (result)
                result = await DisplayAlert("DELETE ALL DATA?", "ARE YOU SURE?", "YES", "CANCEL");
                if (result)
                {
                    using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                    {
                        conn.DeleteAll<Term>();
                        conn.DeleteAll<Course>();
                        conn.DeleteAll<Assessment>();
                    }

                    await DisplayAlert("Notice", "Please exit app to finish resetting data", "Ok");
                }
        }
    }
}

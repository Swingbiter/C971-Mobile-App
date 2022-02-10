using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using SQLite;
using C971_Mobile_App.AddEditForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace C971_Mobile_App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CourseDetailPage : ContentPage
    {
        private Course _course;
        public CourseDetailPage(Course course)
        {
            InitializeComponent();
            _course = course;
        }

        protected override void OnAppearing()
        {
            lbl_CourseName.Text = $"Course: {_course.CourseName}";
            lbl_Status.Text = $"Status: {_course.Status}";
            lbl_Start.Text = $"Start Date: {_course.StartDate.ToString("MM/dd/yyyy")}";
            lbl_End.Text = $"End Date: {_course.EndDate.ToString("MM/dd/yyyy")}";
            lbl_Instructor.Text = $"Instructor: {_course.Instructor}";
            lbl_Phone.Text = $"Phone: {_course.InstructorPhone}";
            lbl_Email.Text = $"Email: {_course.InstructorEmail}";
            lbl_Notifications.Text = $"Notifications: {_course.Notification.ToString()}";
            lbl_Notes.Text = $"Notes: {_course.Notes}";
            base.OnAppearing();
        }

        private void btn_assessments_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AssessmentsPage(_course));
        }

        private void btn_edit_course_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddEditCourse(_course));
        }

        private async void btn_del_course_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayAlert("Delete Term?", "Are you sure?", "Yes", "Cancel");
            if (result)
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                {
                    conn.Delete(_course);
                }
                await Navigation.PopToRootAsync();
            }
        }

        private void btn_share_notes_Clicked(object sender, EventArgs e)
        {
            Share.RequestAsync(new ShareTextRequest
            {
                Text = _course.Notes,
                Title = "Notes"
            });
        }
    }
}
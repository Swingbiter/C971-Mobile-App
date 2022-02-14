using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using C971_Mobile_App.AddEditForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace C971_Mobile_App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AssessmentsPage : ContentPage
    {
        private Course _course;
        public AssessmentsPage(Course course)
        {
            InitializeComponent();
            _course = course;
        }

        protected override void OnAppearing()
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                var assessments = conn.Query<Assessment>($"SELECT * FROM Assessment WHERE Course = {_course.Id}").ToList();
                listView_Assessments.ItemsSource = assessments;
                if (assessments.Count >= 2) 
                {
                    btn_add_assess.IsVisible = false;
                }
            }
            lbl_CourseName.Text = _course.CourseName;


            base.OnAppearing();
        }

        private void listView_Assessments_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Assessment assessment = (Assessment)e.Item;

            Navigation.PushAsync(new AddEditAssessment(assessment));
        }

        private void btn_add_assess_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddEditAssessment(_course));
        }
    }
}
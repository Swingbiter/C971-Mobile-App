using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using C971_Mobile_App.AddEditForms;

namespace C971_Mobile_App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CoursesPage : ContentPage
    {
        private Term _term;
        public CoursesPage(Term term)
        {
            InitializeComponent();
            _term = term;
            lbl_Term.Text = $"{term.Title}: ";
            lbl_DateRange.Text = $"{term.StartDate:MM/dd/yyyy} - {term.EndDate:MM/dd/yyyy}";
        }

        protected override void OnAppearing()
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                var courses = conn.Query<Course>($"SELECT * FROM Course WHERE Term = {_term.Id}").ToList();
                listView_Courses.ItemsSource = courses;
            }
            base.OnAppearing();
        }

        private void listView_Courses_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Course course = (Course)e.Item;
            Navigation.PushAsync(new CourseDetailPage(course));
        }

        private void btn_edit_term_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddEditTerm(_term));
        }

        private async void btn_del_term_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayAlert("Delete Term?", "Are you sure?", "Yes", "Cancel");
            if (result)
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                {
                    conn.Delete(_term);
                }
                await Navigation.PopToRootAsync();
            }
        }

        private void btn_add_course_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddEditCourse(_term));
        }
    }
}
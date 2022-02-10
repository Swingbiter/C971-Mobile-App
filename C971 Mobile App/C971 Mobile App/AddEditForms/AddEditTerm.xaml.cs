using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace C971_Mobile_App.AddEditForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEditTerm : ContentPage
    {
        private bool edit_mode = false;
        private Term _term;
        public AddEditTerm()
        {
            InitializeComponent();
            lbl_add_edit.Text = "Add";
            btn_add_save.Text = "Add New Term";
        }
        public AddEditTerm(Term term)
        {
            InitializeComponent();
            edit_mode = true;
            _term = term;
            lbl_add_edit.Text = "Edit";
            entry_title.Text = _term.Title;
            datepicker_start.Date = _term.StartDate;
            datepicker_end.Date = _term.EndDate;
            btn_add_save.Text = "Save";
        }

        private void btn_add_save_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(entry_title.Text))
            {
                DisplayAlert("Invalid Input", "Term title can not be empty", "OK");
                return;
            }

            if ((datepicker_start.Date > datepicker_end.Date) || (datepicker_start.Date == datepicker_end.Date))
            {
                DisplayAlert("Date Error", "Starting Date can't be after or equal to end date", "OK");
                return;
            }

            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                if (edit_mode)
                {
                    _term.Title = entry_title.Text;
                    _term.StartDate = datepicker_start.Date;
                    _term.EndDate = datepicker_end.Date;
                    conn.Update(_term);
                    Navigation.PopToRootAsync();
                }   
                else
                {
                    Term new_term = new Term();
                    new_term.Title = entry_title.Text;
                    new_term.StartDate = datepicker_start.Date;
                    new_term.EndDate = datepicker_end.Date;
                    conn.Insert(new_term);
                    Navigation.PopToRootAsync();
                }
            }
        }
    }
}
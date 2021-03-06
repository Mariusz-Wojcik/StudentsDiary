﻿using StudentsDiary.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace StudentsDiary
{
    public partial class Main : Form
    {
        private FileHelper<List<Student>> _fileHelper = new FileHelper<List<Student>>(Program.FilePath);

        public bool IsMaximize
        {
            get
            {
                return Settings.Default.IsMaximize;
            }
            set
            {
                Settings.Default.IsMaximize = value;
            }
        }

        public Main()
        {
            InitializeComponent();
            RefreshDiary();
            SetColumnHeader();
            InitComboboxGroups();
            if (IsMaximize)
                WindowState = FormWindowState.Maximized;

        }

        private void InitComboboxGroups()
        {
            var _groups = new List<Group>
            {
            new Group { Id = 0, Name = "Wszystkie" },
            new Group { Id = 1, Name = "1A" },
            new Group { Id = 2, Name = "1B" },
            new Group { Id = 3, Name = "2A" },
            new Group { Id = 4, Name = "2B" }
            };
            cbbGroupFilter.DataSource = _groups;
            cbbGroupFilter.DisplayMember = "Name";
            cbbGroupFilter.ValueMember = "Id";
        }

        private void RefreshDiary()
        {
            var students = _fileHelper.DeserializeFromFile();
            var selectedGroup = Convert.ToInt32(cbbGroupFilter.SelectedValue);

            if (selectedGroup > 0)
            {
                students = students.Where(x =>
                    x.GroupId == cbbGroupFilter.Text.ToString()).ToList();
            }
            dgvDiary.DataSource = students;

        }

        private void SetColumnHeader()
        {
            dgvDiary.Columns[0].HeaderText = "Numer";
            dgvDiary.Columns[1].HeaderText = "Imię";
            dgvDiary.Columns[2].HeaderText = "Nazwisko";
            dgvDiary.Columns[3].HeaderText = "Uwagi";
            dgvDiary.Columns[4].HeaderText = "Matematyka";
            dgvDiary.Columns[5].HeaderText = "Technologia";
            dgvDiary.Columns[6].HeaderText = "Fizyka";
            dgvDiary.Columns[7].HeaderText = "Język polski";
            dgvDiary.Columns[8].HeaderText = "Język obcy";
            dgvDiary.Columns[9].HeaderText = "Zajęcia dodatkowe";
            dgvDiary.Columns[10].HeaderText = "Grupa";
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            var addEditStuden = new AddEdditStudent();
            addEditStuden.FormClosing += AddEditStuden_FormClosing;
            addEditStuden.ShowDialog();
        }

        private void AddEditStuden_FormClosing(object sender, FormClosingEventArgs e)
        {
            RefreshDiary();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvDiary.SelectedRows.Count == 0)
            {
                MessageBox.Show("Zaznacz ucznia którego dane chcesz edytować");
                return;
            }

            var addEditStuden = new AddEdditStudent(Convert.ToInt32(dgvDiary.SelectedRows[0].Cells[0].Value));
            addEditStuden.FormClosing += AddEditStuden_FormClosing;
            addEditStuden.ShowDialog();

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvDiary.SelectedRows.Count == 0)
            {
                MessageBox.Show("Zaznacz ucznia którego dane chcesz usunąć");
                return;
            }

            var selectedStudent = dgvDiary.SelectedRows[0];
            var confirmDelete =
                MessageBox.Show($"Czy na pewno chcesz usunąć ucznia " +
                $"{(selectedStudent.Cells[1].Value.ToString() + " " + selectedStudent.Cells[2].Value.ToString()).Trim()}",
                "Usuwanie ucznia",
                MessageBoxButtons.OKCancel);

            if (confirmDelete == DialogResult.OK)
            {
                DeleteStudent(Convert.ToInt32(selectedStudent.Cells[0].Value));
                RefreshDiary();
            }
        }

        private void DeleteStudent(int id)
        {
            var students = _fileHelper.DeserializeFromFile();
            students.RemoveAll(x => x.Id == id);
            _fileHelper.SerializeToFile(students);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshDiary();
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
                IsMaximize = true;
            else
                IsMaximize = false;
            Settings.Default.Save();
        }
    }
}

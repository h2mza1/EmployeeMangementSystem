import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { History } from 'src/app/Models/historyModel';
import { HistoryService } from 'src/app/Services/history.service';
import { DeleteComponent } from '../../Shared/delete/delete.component';

@Component({
  selector: 'app-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.css'],
})
export class IndexComponent implements OnInit {
  historyList: History[] = [];      
  filteredHistory: History[] = [];  
  searchItem:string=""
  filters = {
    employee: false,
    department: false,
    attendance: false
  };

  constructor(private service: HistoryService , private dialog:MatDialog) {}

  ngOnInit(): void {
    this.service.getAll().subscribe({
      next: (res) => {
     
        this.historyList = [
          ...res.Employee,
          ...res.Department,
          ...res.Attendance
        ];

        this.filteredHistory = [...this.historyList];
        console.log('History List:', this.historyList);
      },
      error: (err) => {
        console.error('Error loading history:', err);
      },
    });
  }

  applyFilters() {
    const { employee, department, attendance } = this.filters;
    const term = this.searchItem.toLowerCase().trim();

    this.filteredHistory = this.historyList.filter((item) => {
      const type = item.EntityType?.toLowerCase();
      const matchesType =
        (!employee && !department && !attendance) ||
        (employee && type === 'employee') ||
        (department && type === 'department') ||
        (attendance && type === 'attendance');

      const matchesSearch =
        !term ||
        item.Name?.toLowerCase().includes(term) ||
        item.Description?.toLowerCase().includes(term) ||
        item.ExtraInfo?.toLowerCase().includes(term);

      return matchesType && matchesSearch;
    });
  }
  restore(model: History) {
    this.service.restore(model).subscribe({
      next: () => {
        this.historyList = this.historyList.filter((x) => x.Id !== model.Id);
        this.filteredHistory = this.filteredHistory.filter((x) => x.Id !== model.Id);

      },
      error: (err) => {
        console.error(err);
      },
    });
  }
  openDelete(model:History)
  {
    const dialogRef = this.dialog.open(DeleteComponent,
      {
        width:"400px"
      }
    )
    dialogRef.afterClosed()
    .subscribe(
      {next:(res)=>
        {
          if(res==true)
          this.delete(model);
        }}
    )
  }
  delete(model:History)
  {
    this.service.delete(model)
    .subscribe(
      {
        next:(res)=>
        {
          this.filteredHistory = this.filteredHistory.filter(x=> x.Id!=model.Id)
        }
      }
    )
  }
}

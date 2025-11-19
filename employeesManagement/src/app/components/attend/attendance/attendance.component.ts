import {
  Component,
  Input,
  OnChanges,
  OnInit,
  SimpleChanges,
} from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Attendance } from 'src/app/Models/attendance';
import { AttendService } from 'src/app/Services/attend.service';
import { DeleteComponent } from '../../Shared/delete/delete.component';
import { EntryComponent } from '../entry/entry.component';

@Component({
  selector: 'app-attendance',
  templateUrl: './attendance.component.html',
  styleUrls: ['./attendance.component.css'],
})
export class AttendanceComponent implements OnInit, OnChanges {
  @Input() AttenList: Attendance[] = [];
  @Input() date!: string;
  @Input() absent: boolean = false;
  @Input() present: boolean = false;
  @Input() searchItem: string = '';

  private originalList: Attendance[] = [];

  constructor(
    private _attendService: AttendService,
    private _dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadEmployee();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (!this.date && !this.absent && !this.present && !this.searchItem) {
      this.loadEmployee();
      return;
    }

    if (changes['date'] && this.date) {
      const selectedDate = new Date(this.date);
      this._attendService.getAttendanceByDate(selectedDate).subscribe((res) => {
        this.originalList = res;
        this.applyFilters();
      });
    } else {
      this.applyFilters();
    }
  }

  loadEmployee() {
    this._attendService.getAttendanceForThisDay().subscribe((res) => {
      this.originalList = res;
      this.AttenList = [...this.originalList];
    });
  }

  applyFilters() {
    let filtered = [...this.originalList];

    if (this.searchItem?.trim()) {
      filtered = filtered.filter((a) =>
        a.EmployeeName?.toLowerCase().includes(this.searchItem.toLowerCase())
      );
    }

    if (this.absent || this.present) {
      filtered = filtered.filter(
        (a) =>
          (this.present && a.Status === true) ||
          (this.absent && a.Status === false)
      );
    }

    this.AttenList = filtered;
  }

  sendDate(date: Date) {
    this._attendService
      .getAttendanceByDate(date)
      .subscribe((res) => (this.AttenList = res));
  }

  openDialog(id: number) {
    const dialogRef = this._dialog.open(DeleteComponent, { width: '400px' });

    dialogRef.afterClosed().subscribe((res) => {
      if (res) {
        this._attendService.delete(id).subscribe({
          next: () => {
            this.AttenList = this.AttenList.filter((item) => item.Id !== id);
          },
          error: (err) => {},
        });
      }
    });
  }

  openDialogAdd(id?: number) {
    const dialogRef = this._dialog.open(EntryComponent, {
      width: '40%',
      data: { id: id ?? 0 },
    });

    dialogRef.afterClosed().subscribe({
      next: (res) => {
        if (res) {
          this.loadEmployee();
        }
      },
    });
  }
}

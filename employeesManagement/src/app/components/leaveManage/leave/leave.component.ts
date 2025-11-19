import { Component, OnInit } from '@angular/core';
import { Leave } from '../../../Models/leave';
import { LeaveService } from '../../../Services/leave.service';
import { MatDialog } from '@angular/material/dialog';
import { DeleteComponent } from '../../Shared/delete/delete.component';
import { RequestStatus } from '../../empHome/home/home.component';
@Component({
  selector: 'app-leave',
  templateUrl: './leave.component.html',
  styleUrls: ['./leave.component.css'],
})
export class LeaveComponent implements OnInit {

  reqList: Leave[] = [];        // list shown on UI
  originalList: Leave[] = [];   // backup list (unfiltered)
  reqStatus:string[] = ['Approved','Rejected','Pinding']
  item: string = "";
  selectedStatus:string=""
  constructor(private context: LeaveService, private dialog: MatDialog) {}

  loadAllReq() {
    this.context.getAll().subscribe({
      next: (res) => {
        this.originalList = res;     // keep original data
        this.reqList = res;          // render to UI
      },
      error: (err) => {},
    });
  }

  ngOnInit(): void {
    this.loadAllReq();
  }

  search() {
    const term = this.item.toLowerCase();

    this.reqList = this.originalList.filter(x =>
      x.EmployeeName?.toLowerCase().includes(term) ||
      x.ApprovedByName?.toLowerCase().includes(term)
    );
  }

  approve(id: number) {
    this.context.approve(id).subscribe(() => this.loadAllReq());
  }

  reject(id?: number) {
    this.context.reject(id).subscribe(() => this.loadAllReq());
  }

  delete(id?: number) {
    const dialogRef = this.dialog.open(DeleteComponent, { width: 'auto' });

    dialogRef.afterClosed().subscribe((res) => {
      if (res === true && id) {
        this.context.delete(id).subscribe(() => {
          this.reqList = this.reqList.filter((x) => x.Id !== id);
          this.originalList = this.originalList.filter((x) => x.Id !== id);
        });
      }
    });
  }
  openAddOrUpdate() {}
filter() {
  if (this.selectedStatus === "") {
    this.reqList = this.originalList;
  } 
  else {
    this.reqList = this.originalList.filter(
      x => x.Status === this.selectedStatus
    );
  }
}

}

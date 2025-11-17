import { Component, Inject, OnInit, Optional } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { SalaryDetail } from 'src/app/Models/salary-detail';
import { SalaryService } from 'src/app/Services/salary.service';

@Component({
  selector: 'app-salarydetails',
  templateUrl: './salarydetails.component.html',
  styleUrls: ['./salarydetails.component.css']
})
export class SalarydetailsComponent implements OnInit {
salList:SalaryDetail[]=[]
  constructor(
    private salService:SalaryService,
    @Optional() private dialogRef: MatDialogRef<SalarydetailsComponent>,
    @Inject(MAT_DIALOG_DATA) public data:number  ) { }

  ngOnInit(): void {
    this.salService.getSalaryDetails(this.data)
    .subscribe(
      {
        next:(res)=>
        {
          this.salList=res
        }
      }
    )
  }

}

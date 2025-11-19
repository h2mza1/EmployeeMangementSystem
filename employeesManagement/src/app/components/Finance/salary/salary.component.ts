  import { Component, OnChanges, OnInit, SimpleChanges } from '@angular/core';
  import { MatDialog } from '@angular/material/dialog';
  import { Salary } from 'src/app/Models/salary';
  import { SalaryService } from 'src/app/Services/salary.service';
  import { EntryComponent } from '../entry/entry.component';
  import * as XLSX from 'xlsx';
  import { saveAs } from 'file-saver';
  import { DeleteComponent } from '../../Shared/delete/delete.component';
  import { Employee } from 'src/app/Models/employee';
  import { EmployeeService } from 'src/app/Services/employee.service';
  import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import { ChangeDetectorRef ,NgZone} from '@angular/core';

  @Component({
    selector: 'app-salary',
    templateUrl: './salary.component.html',
    styleUrls: ['./salary.component.css'],
  })
  export class SalaryComponent implements OnInit,OnChanges {
    constructor(private context: SalaryService, private dialog: MatDialog,private empService:EmployeeService,
        private cd: ChangeDetectorRef ,  private zone: NgZone,


    ) {}

    salaryList: Salary[] = [];
    totalMonth: number = 0;
    date: Date = new Date();
    empList:Employee[]=[]
    empId:number=0
    selectedSalary: Salary | null = null;
    today = new Date();
    ngOnInit(): void {
      this.context.getSalaryForThisMonth().subscribe({
        next: (res) => {
          this.salaryList = res.Salaries;
          this.totalMonth = res.sumSal;
        },
        error: (err) => {
        },
      });
      this.empService.getAllEmployees()
      .subscribe
      ((res)=> this.empList=res)
    }
    ngOnChanges(): void {
      this.filter()
    }
    addAll() {
      this.context.addSalaryForAllEmp().subscribe({
        next: (res) => {
          this.salaryList = res;

        },
        error: (err) => {
        },
      });
    }
    openDialog() {
      const dialogRef = this.dialog.open(EntryComponent, {
        width: 'auto',
      });
    }

    getByDate(data: Date) {
      const selectedDate = new Date(this.date);
      this.context.getSalriesByDate(selectedDate).subscribe({
        next: (res) => {
          this.salaryList = res.salaries;
          this.totalMonth = res.sumSal;
        },
        error: (err) => {
          this.salaryList = [];
        },
      });
    }
    // exportToExcel() {
    //   const total = [
    //     ...this.salaryList,
    //     {
    //       employeeName: 'TOTAL',

    //       netSalary: this.totalMonth,
    //     },
    //   ];

    //   const worksheet = XLSX.utils.json_to_sheet(total);

    //   const totalRowIndex = total.length ;

    //   const columns = ['A', 'B', 'C', 'D', 'E', 'F'];
    //   columns.forEach((col) => {
    //     const cell = worksheet[`${col}${totalRowIndex}`];
    //     if (cell) {
    //       cell.s = {
    //         font: { bold: true },
    //         fill: {
    //           fgColor: { rgb: 'FFFF99' },
    //         },
    //         alignment: { horizontal: 'center' },
    //       };
    //     }
    //   });

    //   const workbook = XLSX.utils.book_new();
    //   XLSX.utils.book_append_sheet(workbook, worksheet, 'Salaries');

    //   const excelBuffer: any = XLSX.write(workbook, {
    //     bookType: 'xlsx',
    //     type: 'array',
    //     cellStyles: true, // ضروري لتفعيل التنسيق
    //   });

    //   const fileData = new Blob([excelBuffer], {
    //     type:
    //       'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8',
    //   });

    //   saveAs(fileData, `Salaries_${new Date().toISOString().slice(0, 10)}.xlsx`);
    // }
    exportToExcel() {
      const totalNetSalary = this.totalMonth;

      const dataForExcel = [
        ...this.salaryList,
        {
          Id: '',
          EmployeeId: '',
          Month: '',
          Year: '',
          EmployeeName: 'TOTAL',
          BaseSalary: '',
          Deduction: '',
          NetSalary: totalNetSalary,
        },
      ];

      const worksheet = XLSX.utils.json_to_sheet(dataForExcel);

      const workbook = XLSX.utils.book_new();
      XLSX.utils.book_append_sheet(workbook, worksheet, 'Salaries');

      const excelBuffer: any = XLSX.write(workbook, {
        bookType: 'xlsx',
        type: 'array',
      });

      const fileData = new Blob([excelBuffer], {
        type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8',
      });

      saveAs(fileData, `Salaries_${this.date}.xlsx`);
    }

    openDelete(id: number) {
      const daialogRef = this.dialog.open(DeleteComponent, {
        width: '400px',
      });
      daialogRef.afterClosed().subscribe((res) => {
        if (res) {
          this.delete(id);
          this.salaryList.filter((x) => x.Id == id);
        }
      });
    }
    delete(id: number) {
      this.context.delete(id).subscribe({
        next: (res) => {
        },
        error: (err) => {
        },
      });
    }
  filter() {
  if (this.empId != 0 && this.date) {
    this.context.getSalaryByUserAndDate(this.empId, this.date).subscribe({
      next: (res) => {
        this.salaryList = res;
        this.totalMonth = this.salaryList.reduce((sum, s) => sum + s.NetSalary, 0);
      },
      error: (err) => {}
    });
  }

  else if (this.empId != 0 && !this.date) {
    this.context.getSalaryByUserAndDate(this.empId).subscribe({
      next: (res) => {
        this.salaryList = res;
        this.totalMonth = this.salaryList.reduce((sum, s) => sum + s.NetSalary, 0);
      },
      error: (err) => {}
    });
  }

  else if (this.empId==0 && this.date) {
    this.context.getSalaryByUserAndDate(0,this.date)
    .subscribe({
      next: (res) => {
        this.salaryList = res;
        this.totalMonth = this.salaryList.reduce((sum, s) => sum + s.NetSalary, 0);
      },
      error: (err) =>{}
    });
  }

  else {
    this.context.getSalaryForThisMonth().subscribe({
      next: (res) => {
        this.salaryList = res.Salaries;
        this.totalMonth = res.sumSal;
      },
      error: (err) => {}
    });
  }
}


generateEmployeePDF(salary: Salary) {
  this.selectedSalary = salary;
  this.today = new Date();
  this.cd.detectChanges();

  this.zone.runOutsideAngular(() => {
    setTimeout(() => {
      const element = document.getElementById('salaryTemplate');
      if (!element) {
        return;
      }

      element.style.visibility = 'visible';
      element.style.position = 'static';
      element.style.left = '0';

      html2canvas(element, { scale: 2, useCORS: true, backgroundColor: "#ffffff" })
        .then(canvas => {
          const imgData = canvas.toDataURL('image/png');
          const pdf = new jsPDF('p', 'mm', 'a4');
          const pdfWidth = pdf.internal.pageSize.getWidth();
          const pdfHeight = (canvas.height * pdfWidth) / canvas.width;

          pdf.addImage(imgData, 'PNG', 0, 0, pdfWidth, pdfHeight);
          pdf.save(`${salary.EmployeeName}_Salary_${salary.Month}-${salary.Year}.pdf`);
        })
        .catch(err => console.error('error on PDF:', err))
        .finally(() => {
          element.style.visibility = 'hidden';
          element.style.position = 'absolute';
          element.style.left = '-9999px';
        });
    }, 500);
  });
}



  }

import { Component, OnInit } from '@angular/core';
import { Leave } from '../Models/leave';
import { LeaveService } from '../Services/leave.service';

@Component({
  selector: 'app-leave',
  templateUrl: './leave.component.html',
  styleUrls: ['./leave.component.css']
})
export class LeaveComponent implements OnInit {

  reqList:Leave[]=[]
  constructor(private context:LeaveService) { }

  ngOnInit(): void {
    this.context.getAll()
    .subscribe(
      {
        next:(res)=>
        {
          this.reqList = res  
          console.log(this.reqList)
        },
        error:(err)=>
        {
          console.log(err);
        }
      }
    )
  }
  openAddOrUpdate(){};
approve(id:number)
{
  this.context.approve(id)
  .subscribe(res=> console.log(res))
}
delete(id?:number)
{
   this.context.delete(id)
  .subscribe(res=> console.log(res))
}
reject(id?:number)
{
  this.context.reject(id)
  .subscribe(
    res=> console.log(res)
  )
}
}

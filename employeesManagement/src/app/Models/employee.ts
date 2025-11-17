export interface Employee {
    Id : number,
    Name : string,
    Email:string,
    Phone:string,
    DeptId:number,
    DeptName:string,
    Password:string,
    StartDate:string | Date,
    EndDate:string | Date,
    IsDeleted:boolean,
    RoleId:number,
    RoleName:string,
    Salary : number,
    expectedWorkHours : number ,
    workingHours : number 
}

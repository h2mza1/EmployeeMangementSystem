export interface Leave {
    Id:number | null,
    EmployeeName:string | null,
    EmployeeId:number | null,
    Type:string | null,
    Status:string | null,
    StartDate : Date | null,
    EndDate : Date | null,
    FromTime : Date | null,
    ToTime : Date | null,
    Reason : string | null,
    ApprovedByName : string | null,
    VacationId : number
}

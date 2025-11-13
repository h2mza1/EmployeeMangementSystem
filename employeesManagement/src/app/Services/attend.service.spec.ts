import { TestBed } from '@angular/core/testing';

import { AttendService } from './attend.service';

describe('AttendService', () => {
  let service: AttendService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AttendService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

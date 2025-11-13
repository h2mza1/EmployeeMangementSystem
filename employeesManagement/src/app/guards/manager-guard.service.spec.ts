import { TestBed } from '@angular/core/testing';

import { ManagerGuardService } from './manager-guard.service';

describe('ManagerGuardService', () => {
  let service: ManagerGuardService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ManagerGuardService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

import { TestBed } from '@angular/core/testing';

import { CashDeskService } from './cash-desk.service';

describe('CashDeskService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: CashDeskService = TestBed.get(CashDeskService);
    expect(service).toBeTruthy();
  });
});

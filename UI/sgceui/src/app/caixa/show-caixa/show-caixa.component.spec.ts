import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowCaixaComponent } from './show-caixa.component';

describe('ShowCaixaComponent', () => {
  let component: ShowCaixaComponent;
  let fixture: ComponentFixture<ShowCaixaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ShowCaixaComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ShowCaixaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

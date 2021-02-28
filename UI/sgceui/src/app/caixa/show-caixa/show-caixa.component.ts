import { Component, OnInit } from '@angular/core';
import { SharedService} from 'src/app/shared.service';

@Component({
  selector: 'app-show-caixa',
  templateUrl: './show-caixa.component.html',
  styleUrls: ['./show-caixa.component.css']
})
export class ShowCaixaComponent implements OnInit {

  constructor(private service:SharedService) { }
  
  CaixaStatusList: any=[];

  ngOnInit(): void {
    this.refreshCaixaStatusList();
  }

//async operation
  refreshCaixaStatusList(){
    this.service.getCaixaStatus().subscribe(
      data => {
      this.CaixaStatusList=data;
    });
  }

}

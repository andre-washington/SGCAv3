import { Component, OnInit } from '@angular/core';
import { SharedService} from 'src/app/shared.service';




@Component({
  selector: 'app-saque',
  templateUrl: './saque.component.html',
  styleUrls: ['./saque.component.css']
})
export class SaqueComponent implements OnInit {
  CaixaAtivoList: any=[];
  selectedValue: Int16Array;
  selectedCaixa: Int16Array;

  
  constructor( private service:SharedService) { }

  ngOnInit(): void {
    this.refreshCaixaAtivoList();
  }

  refreshCaixaAtivoList(){
    this.service.getCaixaAtivo().subscribe(
      data => {
       //aqui vem do get
      this.CaixaAtivoList = data;
      this.selectedCaixa = this.CaixaAtivoList[0].CaixaId;
      console.log( "loggg" +this.CaixaAtivoList);
    });
  }
}

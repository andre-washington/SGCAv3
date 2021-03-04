import { Component, OnInit } from '@angular/core';
import { SharedService} from 'src/app/shared.service';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { SignalRService } from '../services/signal-r.service';


import { Saque } from 'src/app/saque/saque';

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type':  'application/json'    
  })
};

@Component({
  selector: 'app-saque',
  templateUrl: './saque.component.html',
  styleUrls: ['./saque.component.css']
})

export class SaqueComponent implements OnInit {
  CaixaAtivoList: any=[];
  idCaixa: Int16Array;
  selectedCaixa: Int16Array;
  valorSaque: Int16Array;
  saque: Saque;

  
  constructor(public signalRService: SignalRService, private service:SharedService, private http: HttpClient) { }
  SaqueStatus: any=[];

  ngOnInit(): void {
    this.refreshCaixaAtivoList();
    this.signalRService.addSaqueStatusListener();
  }

  refreshCaixaAtivoList(){
    this.service.getCaixaAtivo().subscribe(
      data => {
       //aqui vem do get
      this.CaixaAtivoList = data;
      this.selectedCaixa = this.CaixaAtivoList[0].CaixaId;
      console.log( "loggg" +this.CaixaAtivoList);

      console.log("status do saque"+this.signalRService.SaqueSignalStatus);
    });
  }

  onSubmit() {    
    this.http.post<Saque>( 'http://localhost:5000/api/caixa/saque', JSON.stringify({ Caixa: {caixaId: this.idCaixa}, valorSaque: this.valorSaque }), httpOptions)
        .subscribe(result => { }, error => console.error(error));
        // this.SaqueStatus  = this.signalRService.SaqueSignalStatus;
}
}

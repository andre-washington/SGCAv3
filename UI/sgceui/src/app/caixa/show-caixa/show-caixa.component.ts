import { Component, OnInit } from '@angular/core';
import { SharedService} from 'src/app/shared.service';
import { SignalRService } from '../../services/signal-r.service';


@Component({
  selector: 'app-show-caixa',
  templateUrl: './show-caixa.component.html',
  styleUrls: ['./show-caixa.component.css']
})
export class ShowCaixaComponent implements OnInit {

  constructor(public signalRService: SignalRService, private service:SharedService) { }
  
  CaixaStatusList: any=[];

  ngOnInit(): void {
    this.signalRService.startConnection();
    this.signalRService.addTransferChartDataListener();  //captura o evento 
    this.CaixaStatusList = this.signalRService.CaixaSignalList;
    this.refreshCaixaStatusList();
    
  }

//async operation
  refreshCaixaStatusList(){
    this.service.getCaixaStatus().subscribe(
      data => {
      this.CaixaStatusList=data; //aqui vem do get
    });

    this.service.getCaixaSignalStatus().subscribe(
      data => {
       //aqui vem do get
      this.signalRService.CaixaSignalList= data;
    });
  }

}

import {Component, Input, OnInit, ViewEncapsulation} from '@angular/core';
import {Member} from "../../_model/member";

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class MemberCardComponent implements OnInit{
  @Input() member: Member | undefined;

  constructor() {
  }

  ngOnInit(): void {
  }
}

import {Component, Input, Self} from '@angular/core';
import {ControlValueAccessor, FormControl, NgControl} from "@angular/forms";
import {Subscription} from "rxjs";

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.css']
})
export class TextInputComponent implements ControlValueAccessor {
  @Input() label = '';
  @Input() type = '';
  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this;
  }
  registerOnChange(fn: any): void {
  }

  registerOnTouched(fn: any): void {
  }

  writeValue(obj: any): void {
  }

  get control(): FormControl{
    return this.ngControl.control as FormControl
  }

  handleUnchange(event:any){
    console.log(event)
  }
}

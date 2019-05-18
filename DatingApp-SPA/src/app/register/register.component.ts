import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { registerModuleFactory } from '@angular/core/src/linker/ng_module_factory_loader';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  model: any = {};

  @Output() registerMode = new EventEmitter();

  constructor(private auth: AuthService) { }

  ngOnInit() {
  }

  register() {
    this.auth.register(this.model).subscribe(
      () => { console.log('registered');
      },
      error => {
        console.log(error);
      }
    );
  }

  cancel() {
    this.registerMode.emit(false);
    console.log('hit cancel method');
  }
}

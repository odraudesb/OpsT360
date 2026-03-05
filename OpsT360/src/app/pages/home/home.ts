import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { T360HeaderComponent } from '../../components/header/t360-header/t360-header';

@Component({
  selector: 't360-home-page',
  standalone: true,
  imports: [CommonModule, T360HeaderComponent],
  templateUrl: './home.html',
})
export class HomeComponent {
  background = 'assets/images/logoLogin.jpeg';
}

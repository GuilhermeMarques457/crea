import { Component, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-page-header',
  standalone: true,
  imports: [MatIconModule],
  templateUrl: `./page-header.component.html`,
})
export class PageHeaderComponent {
  title = input.required<string>();
  subtitle = input<string>();
  icon = input<string>();
}

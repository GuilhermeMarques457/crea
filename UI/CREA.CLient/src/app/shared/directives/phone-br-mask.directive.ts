import { DestroyRef, Directive, ElementRef, HostListener, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NgControl } from '@angular/forms';
import { formatPhoneBrMask, stripPhoneBr } from '../utils/phone-br';

/** Máscara (XX) XXXX-XXXX ou (XX) XXXXX-XXXX conforme o usuário digita. */
@Directive({
  selector: 'input[appPhoneBrMask]',
  standalone: true,
})
export class PhoneBrMaskDirective implements OnInit {
  private readonly el = inject(ElementRef<HTMLInputElement>);
  private readonly ngControl = inject(NgControl, { optional: true, self: true });
  private readonly destroyRef = inject(DestroyRef);

  ngOnInit(): void {
    const control = this.ngControl?.control;
    if (control) {
      control.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.syncModelToMask());
    }
    queueMicrotask(() => this.syncModelToMask());
  }

  private syncModelToMask(): void {
    const c = this.ngControl?.control;
    if (!c) return;
    const v = c.value;
    if (typeof v !== 'string') return;
    const masked = formatPhoneBrMask(v);
    if (masked !== v) c.setValue(masked, { emitEvent: false });
    const input = this.el.nativeElement;
    if (input.value !== masked) input.value = masked;
  }

  @HostListener('input')
  onInput(): void {
    const input = this.el.nativeElement;
    const caret = input.selectionStart ?? 0;
    const digitsBeforeCaret = stripPhoneBr(input.value.slice(0, caret)).length;

    const digits = stripPhoneBr(input.value).slice(0, 11);
    const masked = formatPhoneBrMask(digits);

    if (this.ngControl?.control && this.ngControl.control.value !== masked) {
      this.ngControl.control.setValue(masked, { emitEvent: true });
    }
    if (input.value !== masked) input.value = masked;

    queueMicrotask(() => {
      let pos = 0;
      let seen = 0;
      for (let i = 0; i < masked.length; i++) {
        if (/\d/.test(masked[i]!)) seen++;
        pos = i + 1;
        if (seen >= digitsBeforeCaret) break;
      }
      input.setSelectionRange(pos, pos);
    });
  }

  @HostListener('blur')
  onBlur(): void {
    this.syncModelToMask();
  }
}

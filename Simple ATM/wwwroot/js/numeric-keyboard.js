class NumericKeyboard {
    constructor(containerSelector, targetInputSelector, options = {}) {
        this.container = document.querySelector(containerSelector);
        this.targetInput = document.querySelector(targetInputSelector);
        if (!this.container || !this.targetInput) {
            throw new Error("NumericKeyboard: Invalid selectors provided.");
        }
        this.maxLength = options.maxLength || null;
        this.format = options.format || null;
        this.onInput = options.onInput || null;
        this.onClear = options.onClear || null;
        this.onRemove = options.onRemove || null;

        this.render();
    }

    render() {
        const keys = `
            <div class="num-row">
                ${this.keyButton(1)}${this.keyButton(2)}${this.keyButton(3)}
            </div>
            <div class="num-row">
                ${this.keyButton(4)}${this.keyButton(5)}${this.keyButton(6)}
            </div>
            <div class="num-row">
                ${this.keyButton(7)}${this.keyButton(8)}${this.keyButton(9)}
            </div>
            <div class="num-row">
                <button class="num-btn remove" type="button">&lt;</button>
                ${this.keyButton(0)}
                <button class="num-btn clear" type="button">C</button>
            </div>
        `;
        this.container.innerHTML = `<div class="numeric-keyboard">${keys}</div>`;
        this.attachEvents();
    }

    keyButton(num) {
        return `<button class="num-btn" data-key="${num}" type="button">${num}</button>`;
    }

    attachEvents() {
        this.container.querySelectorAll(".num-btn").forEach(button => {
            button.addEventListener("click", () => {
                const key = button.dataset.key;
                if (key !== undefined) {
                    this.addDigit(key);
                    if (this.onInput) this.onInput(key);
                }
            });
        });

        this.container.querySelector(".clear").addEventListener("click", () => {
            this.targetInput.value = "";
            if (this.onClear) this.onClear();
        });
        this.container.querySelector(".remove").addEventListener("click", () => {
            let raw = this.targetInput.value.replace(/\D/g, "").slice(0, -1);
            this.targetInput.value = this.formatNumber(raw);
            if (this.onRemove) this.onRemove();
        });
    }
    addDigit(digit) {
        let raw = this.targetInput.value.replace(/\D/g, "");

        if (this.maxLength && raw.length >= this.maxLength) return;

        raw += digit;

        this.targetInput.value = this.formatNumber(raw);
    }

    formatNumber(raw) {
        if (!this.format) return raw;

        const groups = this.format.split("-").map(n => parseInt(n));
        let result = "";
        let index = 0;

        for (const size of groups) {
            if (raw.length <= index) break;
            result += raw.substr(index, size) + "-";
            index += size;
        }

        return result.replace(/-$/, "");
    }
}

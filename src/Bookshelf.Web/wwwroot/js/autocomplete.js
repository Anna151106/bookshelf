(() => {
    const plural = (n, one, few, many) => {
        const mod10 = n % 10;
        const mod100 = n % 100;
        if (mod10 === 1 && mod100 !== 11) return one;
        if (mod10 >= 2 && mod10 <= 4 && (mod100 < 12 || mod100 > 14)) return few;
        return many;
    };

    const language = {
        inputTooShort: args => {
            const n = args.minimum - args.input.length;
            return `Введіть ще ${n} ${plural(n, 'символ', 'символи', 'символів')}`;
        },
        noResults: () => 'Нічого не знайдено',
        searching: () => 'Пошук...',
        errorLoading: () => 'Не вдалося завантажити результати',
        loadingMore: () => 'Завантаження...'
    };

    $('select[data-autocomplete-url]').each((_, element) => {
        const $select = $(element);
        $select.select2({
            theme: 'bootstrap-5',
            width: '100%',
            placeholder: $select.data('placeholder') || '',
            minimumInputLength: 3,
            language,
            ajax: {
                url: $select.data('autocomplete-url'),
                dataType: 'json',
                delay: 300,
                data: params => ({ q: params.term })
            }
        });
    });
})();

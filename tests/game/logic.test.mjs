import { test } from 'node:test';
import assert from 'node:assert/strict';
import { SHELF_Y, HEIGHT, newGame, isCaught, updateBooks, fallSpeed } from '../../src/Bookshelf.Web/wwwroot/js/game/logic.js';

test('book on the shelf is caught, book beside it is not', () => {
    assert.equal(isCaught({ x: 110, y: SHELF_Y - 10 }, 100), true);
    assert.equal(isCaught({ x: 300, y: SHELF_Y - 10 }, 100), false);
});

test('caught book gives 10 points', () => {
    const game = newGame();
    game.books = [{ x: game.shelfX + 10, y: SHELF_Y - 20 }];

    updateBooks(game);

    assert.equal(game.score, 10);
    assert.equal(game.books.length, 0);
});

test('missed book takes one life, last life ends the game', () => {
    const game = newGame();
    game.lives = 1;
    game.books = [{ x: 0, y: HEIGHT }];
    game.shelfX = 300;

    updateBooks(game);

    assert.equal(game.lives, 0);
    assert.equal(game.over, true);
});

test('speed grows every 50 points', () => {
    assert.equal(fallSpeed(0), 2);
    assert.ok(fallSpeed(50) > fallSpeed(40));
});
